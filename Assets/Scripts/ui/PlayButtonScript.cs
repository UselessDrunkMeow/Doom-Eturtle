using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayButtonScript : MonoBehaviour
{
    public string scene = "Prototype Scene";
    public void PlayButtonPress()
    {
        SceneManager.LoadScene(scene);
        print("scene switched");
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
