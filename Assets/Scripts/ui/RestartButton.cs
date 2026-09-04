using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void ButtonPress()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(currentScene);
    }
}
