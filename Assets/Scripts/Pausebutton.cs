using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausebutton : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseUIMenu;
    //[SerializeField] GameObject beatMapPlayerManager;
 
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }        
        }
    }

    void Resume()
    {
        GameIsPaused = false;

        PauseUIMenu.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //beatMapPlayerManager.SetActive(false);

    }

    void Pause()
    {
        GameIsPaused = true;

        PauseUIMenu.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //beatMapPlayerManager.SetActive(true);

    }
}
