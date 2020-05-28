using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class Loadscene : MonoBehaviour

{
    public void Restartgame()
        
  
    {
        
        SceneManager.LoadScene("Playtest Scene 1", LoadSceneMode.Single);
        Time.timeScale = 1;


    }

}
