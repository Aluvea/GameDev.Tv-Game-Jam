using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] Canvas beatMapCanvas;
    [SerializeField] Canvas FPSUICanvas;
    [SerializeField] GameObject beatMapPlayerManager;
    [SerializeField] GameObject pauseCanvas;

    // Start is called before the first frame update
    void Start()
    {
        gameOverCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
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
    }

}
