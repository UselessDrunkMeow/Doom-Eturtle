using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonScript : MonoBehaviour
{
    public string scene = "SampleScene";
    public void PlayButtonPress()
    {
        SceneManager.LoadScene(scene);
    }
}
