using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CardController : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform gridTransform;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject congratulationMessage;
    [SerializeField] private TMP_Text congratulationTmpText;
    [SerializeField] private Text congratulationText;
    [SerializeField] private string winMessage = "Congratulations! You solved the puzzle!";
    [SerializeField] private string mainMenu = "Menu";
    [SerializeField] private bool autoFitGrid = true;
    [SerializeField] private Vector2 minimumCellSize = new Vector2(70f, 70f);

    private readonly List<Sprite> spritePairs = new List<Sprite>();

    private Card firstSelected;
    private Card secondSelected;
    private int matchCounts;
    private bool puzzleCompleted;

    private void Start()
    {
        if (congratulationMessage != null)
        {
            congratulationMessage.SetActive(false);
        }

        PrepareSprites();
        ConfigureGrid();
        CreateCards();
    }

    public void SetSelected(Card card)
    {
        if (puzzleCompleted || card.isSelected)
        {
            return;
        }

        card.Show();

        if (firstSelected == null)
        {
            firstSelected = card;
            return;
        }

        if (secondSelected == null)
        {
            secondSelected = card;
            StartCoroutine(CheckMatching(firstSelected, secondSelected));
            firstSelected = null;
            secondSelected = null;
        }
    }

    private IEnumerator CheckMatching(Card a, Card b)
    {
        yield return new WaitForSeconds(0.3f);

        if (a.iconSprite == b.iconSprite)
        {
            matchCounts++;

            if (matchCounts >= spritePairs.Count / 2)
            {
                HandlePuzzleCompleted();
            }
        }
        else
        {
            a.Hide();
            b.Hide();
        }
    }

    private void PrepareSprites()
    {
        spritePairs.Clear();
        matchCounts = 0;
        puzzleCompleted = false;
        firstSelected = null;
        secondSelected = null;

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null)
            {
                continue;
            }

            spritePairs.Add(sprites[i]);
            spritePairs.Add(sprites[i]);
        }

        ShuffleSprites(spritePairs);
    }

    private void ConfigureGrid()
    {
        if (!autoFitGrid)
        {
            return;
        }

        GridLayoutGroup gridLayout = gridTransform.GetComponent<GridLayoutGroup>();
        RectTransform gridRect = gridTransform as RectTransform;

        if (gridLayout == null || gridRect == null || spritePairs.Count == 0)
        {
            return;
        }

        int columns = Mathf.CeilToInt(Mathf.Sqrt(spritePairs.Count));
        columns = Mathf.Max(2, columns);

        float totalSpacing = gridLayout.spacing.x * Mathf.Max(0, columns - 1);
        float availableWidth = Mathf.Max(0f, gridRect.rect.width - gridLayout.padding.left - gridLayout.padding.right - totalSpacing);
        float cellWidth = availableWidth / columns;
        float cellSize = Mathf.Max(minimumCellSize.x, cellWidth);

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.cellSize = new Vector2(cellSize, Mathf.Max(minimumCellSize.y, cellSize));
    }

    private void CreateCards()
    {
        for (int i = gridTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(gridTransform.GetChild(i).gameObject);
        }

        for (int i = 0; i < spritePairs.Count; i++)
        {
            Card card = Instantiate(cardPrefab, gridTransform);
            card.SetIconSprite(spritePairs[i]);
            card.controller = this;
        }
    }

    private void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];
            spriteList[randomIndex] = temp;
        }
    }

    private void HandlePuzzleCompleted()
    {
        if (puzzleCompleted)
        {
            return;
        }

        puzzleCompleted = true;

        if (congratulationTmpText != null)
        {
            congratulationTmpText.text = winMessage;
        }

        if (congratulationText != null)
        {
            congratulationText.text = winMessage;
        }

        if (congratulationMessage != null)
        {
            congratulationMessage.SetActive(true);
        }
        else
        {
            Debug.Log(winMessage);
        }

        Sequence.Create()
            .Chain(Tween.Scale(gridTransform, Vector3.one * 1.2f, 0.2f, ease: Ease.OutBack))
            .Chain(Tween.Scale(gridTransform, Vector3.one, 0.1f));



    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        if (string.IsNullOrWhiteSpace(mainMenu))
        {
            Debug.LogWarning("Main menu scene name is empty.");
            return;
        }

        SceneManager.LoadScene(mainMenu);
    }

    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif

        }
    }

    
}
