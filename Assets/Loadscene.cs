using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loadscene : MonoBehaviour
{
    public void Restartgame()
    {
        SceneManager.LoadScene("Playtest Scene 2 - Aluvea");
    }

}
