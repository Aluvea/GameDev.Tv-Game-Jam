using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class DoorExit : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] List<EnemyHealth> enemies;
    [SerializeField] EnemyHealth enemyToWaitToDie;
    // Need to make a reference to a door animation
    [SerializeField] int enemiesDied = 0;

    // Start is called before the first frame update
    void Start()
    {
        enemies = FindObjectsOfType<EnemyHealth>().ToList();
        enemyToWaitToDie.Died += EnemyDead;
    }

    private void EnemyDead(EnemyHealth deadEnemy)
    {
        // This method is called whenever an enemy dies
        // Whenever an enemy dies, I want it to add +1 to a counter
        // When counter reaches 11 then execute the door animation

        // A EnemyHealth class called deadEnemy is passed through into this method

        enemiesDied++;
        if (enemiesDied == enemies.Count)
        {
            //Play Door animation OpenDoor();
            door = GameObject.FindWithTag("Gen_Door_01_snaps002");
            door.GetComponent<Animation>().Play("open");
            Debug.Log("All enemies are dead");
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
