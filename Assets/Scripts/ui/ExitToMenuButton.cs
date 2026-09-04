using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenuButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ButtonPress()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
