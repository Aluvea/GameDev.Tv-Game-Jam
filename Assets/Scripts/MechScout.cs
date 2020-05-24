using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechScout : MonoBehaviour
{
    [SerializeField] MechDamageHandler damageHandler;
    [SerializeField] Transform lineOfSightReference;

    [Min(40.0f)]
    [SerializeField] float lineOfSightDegreesRange = 40.0f;
    [SerializeField] float lineOfSightDistance = 25.0f;
    [SerializeField] float lineOfSightFrequencyCheck = 1.0f;
    [SerializeField] LayerMask lineOfSightTestMask;
    [Min(5.0f)]
    [SerializeField] float scoutCallDistance = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Scout());
    }

    private static List<MechScout> mechScouts = new List<MechScout>();

    private void OnEnable()
    {
        mechScouts.Add(this);
    }

    private void OnDisable()
    {
        mechScouts.Remove(this);
        if (mechScouts.Count == 0) mechScouts = new List<MechScout>();
    }

    Vector3 lineOfSightToPlayerDirection;

    float angle;
    IEnumerator Scout()
    {
        while(damageHandler.AttackingPlayer == false)
        {
            yield return new WaitForSeconds(lineOfSightFrequencyCheck);

            if (damageHandler.AttackingPlayer) continue;
            angle = Mathf.Abs(Vector3.Angle(lineOfSightReference.forward, PlayerController.PlayerCamera.transform.position - lineOfSightReference.transform.position));
            Debug.LogWarning(damageHandler.gameObject.name + " angle = " + angle);

            if (Vector3.Distance(PlayerController.PlayerCamera.transform.position, lineOfSightReference.transform.position) >= lineOfSightDistance) continue;
            if (lineOfSightDegreesRange >= angle)
            {
                if(Physics.Linecast(lineOfSightReference.transform.position, PlayerController.PlayerCamera.transform.position, lineOfSightTestMask) == false)
                {
                    AttackPlayer();
                }
            }

            yield return null;
        }

        CallOtherScouts();

        Destroy(this);
    }

    private void CallOtherScouts()
    {
        foreach (MechScout scout in mechScouts)
        {
            if (scout == this) continue;
            if (scout == null) continue;
            if(Vector3.Distance(lineOfSightReference.transform.position, scout.lineOfSightReference.position) <= scoutCallDistance)
            {
                if(Physics.Linecast(lineOfSightReference.transform.position, scout.lineOfSightReference.position, lineOfSightTestMask) == false)
                {
                    scout.AttackPlayer();
                }
            }
        }
    }


    public void AttackPlayer()
    {
        damageHandler.AttackPlayer();
    }
}
