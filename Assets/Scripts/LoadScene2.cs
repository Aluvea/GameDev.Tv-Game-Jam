using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene2 : MonoBehaviour
{
    public void Restartgame()


    {

        SceneManager.LoadScene("Playtest Scene 2 - Aluvea", LoadSceneMode.Single);
        Time.timeScale = 1;


    }

}
