using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject menu; // Assign in inspector
    public bool isShowing;
    PlayerShoot player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerShoot>();
    }
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
                player.enabled = false;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                player.enabled = true;
            }
        }
    }
}
