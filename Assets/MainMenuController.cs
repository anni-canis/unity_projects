using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string easySceneName = "SampleScene";
    [SerializeField] private string hardSceneName = "HardScene";

    
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


    public void PlayEasy()
    {
        LoadScene(easySceneName);
    }

    public void PlayHard()
    {
        LoadScene(hardSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Scene name is empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
