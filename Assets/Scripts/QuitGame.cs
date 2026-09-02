using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitGameFunction()
    {
        Application.Quit();
        print("game has quit");
    }
}
