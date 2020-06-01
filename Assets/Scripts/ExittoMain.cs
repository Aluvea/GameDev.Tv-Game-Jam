using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExittoMain : MonoBehaviour

{
    public void RestartGame()


    {

        SceneManager.LoadScene("Main menu (Current)", LoadSceneMode.Single);
        Time.timeScale = 1;


    }

}