using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Canvas beatMapCanvas;
    [SerializeField] Canvas FPSUICanvas;
    [SerializeField] GameObject beatMapPlayerManager;
    [SerializeField] Canvas mainMenuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        gameOverCanvas.enabled = false;
        beatMapCanvas.enabled = false;
        FPSUICanvas.enabled = false;
        beatMapPlayerManager.SetActive(false);
        mainMenuCanvas.enabled = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleDeath()
    {
        gameOverCanvas.enabled = true;
        beatMapCanvas.enabled = false;
        FPSUICanvas.enabled = false;
        beatMapPlayerManager.SetActive(false);

        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void PlayHandle()
    {
        gameOverCanvas.enabled = false;
        beatMapCanvas.enabled = true;
        FPSUICanvas.enabled = true;
        beatMapPlayerManager.SetActive(true);
        mainMenuCanvas.enabled = false;


    }
}
