using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject menu; // Assign in inspector
    private bool isShowing;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            isShowing = !isShowing;
            menu.SetActive(isShowing);
            if(isShowing == true)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}
