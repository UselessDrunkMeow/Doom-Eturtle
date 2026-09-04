using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public GameObject menu;
    public void pressed()
    {
        menu.SetActive(false);
    }
}
