using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] Canvas beatMapCanvas;
    [SerializeField] Canvas FPSUICanvas;
    [SerializeField] GameObject beatMapPlayerManager;

    // Start is called before the first frame update
    void Start()
    {
        gameOverCanvas.SetActive(false);
        /*beatMapCanvas.enabled = true;
        FPSUICanvas.enabled = true;
        beatMapPlayerManager.SetActive(true);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleDeath()
    {
        gameOverCanvas.SetActive(true);
        beatMapCanvas.enabled = false;
        FPSUICanvas.enabled = false;
        beatMapPlayerManager.SetActive(false);
        Time.timeScale = 0;

    }
    /*public void PlayHandle()
    {
        gameOverCanvas.enabled = false;
        beatMapCanvas.enabled = true;
        FPSUICanvas.enabled = true;
        beatMapPlayerManager.SetActive(true);
    }*/
}
