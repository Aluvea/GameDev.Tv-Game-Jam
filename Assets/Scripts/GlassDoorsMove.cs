using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassDoorsMove : MonoBehaviour
{
    [SerializeField] GameObject outerGlassDoor;
    [SerializeField] GameObject innerGlassDoor;
    [SerializeField] Animator outerAnimator;
    [SerializeField] Animator innerAnimator;
    [SerializeField] BossMech bossMech;
    [SerializeField] AIRoamingController roamingController;
    private bool open = false;
    private bool close = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerHealth>() != null)
        {
            // Play animation to Close outerGlassDoor
            outerAnimator.SetBool("close", true);
            innerAnimator.SetBool("open", true);

            // Play animation to Open innerGlassDoor
            EnableBoss();
            Destroy(this.gameObject);
        }
    }

    private void EnableBoss()
    {
        if(bossMech != null && roamingController != null)
        {
            bossMech.enabled = true;
            roamingController.enabled = true;
        }
    }
}
