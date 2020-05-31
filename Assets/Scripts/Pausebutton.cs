using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausebutton : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject GameOvUI;
 
   
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
        GameOvUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        GameOvUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
