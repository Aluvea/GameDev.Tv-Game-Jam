using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class DoorExit : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] List<EnemyHealth> enemies;
    [SerializeField] Animator doorAnimator;
    [SerializeField] AudioSource doorSource;
    [SerializeField] AudioClip doorOpenSFX;

    private bool open = false;
    private int enemiesDied = 0;

    // Start is called before the first frame update
    void Start()
    {
        EnemyHealth MechWarrior0 = enemies[0];
        MechWarrior0.Died += EnemyHealth_Died;

        EnemyHealth MechWarrior1 = enemies[1];
        MechWarrior1.Died += EnemyHealth_Died;

        EnemyHealth MechWarrior2 = enemies[2];
        MechWarrior2.Died += EnemyHealth_Died;

        EnemyHealth MechWarrior3 = enemies[3];
        MechWarrior3.Died += EnemyHealth_Died;

        EnemyHealth MechWarrior4 = enemies[4];
        MechWarrior4.Died += EnemyHealth_Died;

        EnemyHealth CyberBug0 = enemies[5];
        CyberBug0.Died += EnemyHealth_Died;

        EnemyHealth CyberBug1 = enemies[6];
        CyberBug1.Died += EnemyHealth_Died;

        EnemyHealth CyberBug2 = enemies[7];
        CyberBug2.Died += EnemyHealth_Died;

        EnemyHealth CyberBug3 = enemies[8];
        CyberBug3.Died += EnemyHealth_Died;

        EnemyHealth CyberBug4 = enemies[9];
        CyberBug4.Died += EnemyHealth_Died;

        EnemyHealth CyberBug5 = enemies[10];
        CyberBug5.Died += EnemyHealth_Died;
    }

    private void EnemyHealth_Died(EnemyHealth deadEnemy)
    {
        enemiesDied++;

        if (enemiesDied == enemies.Count)
        {
            Debug.Log("All enemies are dead" + " and Count is " + enemies.Count);

            doorSource.PlayOneShot(doorOpenSFX);
            Debug.Log("Sound should be playing");
            doorAnimator.SetBool("open", true);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
