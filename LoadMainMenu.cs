using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
