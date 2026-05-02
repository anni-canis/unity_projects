using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private float flipDuration = 0.2f;

    public Sprite hiddenIconSprite;
    public Sprite iconSprite;

    public bool isSelected;

    public CardController controller;
    Coroutine flipCoroutine;

    public void OnCardClick()
    {
        controller.SetSelected(this);
    }

    public void SetIconSprite(Sprite sp)
    {
        iconSprite = sp;
    }

    public void Show()
    {
        StartFlip(new Vector3(0f, 180f, 0f), iconSprite, true);
    }

    public void Hide()
    {
        StartFlip(new Vector3(0f, 0f, 0f), hiddenIconSprite, false);
    }

    void StartFlip(Vector3 targetRotation, Sprite targetSprite, bool selectedState)
    {
        if (flipCoroutine != null)
        {
            StopCoroutine(flipCoroutine);
        }

        flipCoroutine = StartCoroutine(FlipRoutine(targetRotation, targetSprite, selectedState));
    }

    IEnumerator FlipRoutine(Vector3 targetRotation, Sprite targetSprite, bool selectedState)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float elapsed = 0f;
        bool spriteSwapped = false;

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / flipDuration);

            transform.rotation = Quaternion.Lerp(startRotation, endRotation, progress);

            if (!spriteSwapped && progress >= 0.5f)
            {
                iconImage.sprite = targetSprite;
                spriteSwapped = true;
            }

            yield return null;
        }

        transform.rotation = endRotation;
        iconImage.sprite = targetSprite;
        isSelected = selectedState;
        flipCoroutine = null;
    }
}
