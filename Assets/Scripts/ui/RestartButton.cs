using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public static void QuerySceneInfo(Scene scene)
    {
        Debug.Log("Scene name: " + scene.name);
        Debug.Log("Scene path: " + scene.path);
        Debug.Log("Scene build index: " + scene.buildIndex);
        Debug.Log("Scene is dirty: " + scene.isDirty);
        Debug.Log("Scene is loaded: " + scene.isLoaded);
        Debug.Log("Scene root count: " + scene.rootCount);
    }
    public void ButtonPress()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        QuerySceneInfo(currentScene);
        SceneManager.SetActiveScene(currentScene);
    }
}
