using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneByIndex : MonoBehaviour
{

    private void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
    // General method to load scenes based on build index
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }
}