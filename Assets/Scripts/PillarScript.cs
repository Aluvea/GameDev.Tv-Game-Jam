using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarScript : MonoBehaviour
{
    [SerializeField] Transform [] gameObjectsToAddRigidBodiesTo;
    [SerializeField] string ChangeThemToLayer = "DeadEnemies";

    public void DestructPillars()
    {
        foreach (Transform transformRefs in gameObjectsToAddRigidBodiesTo)
        {
            if (transformRefs == null) continue;
            transformRefs.gameObject.AddComponent<Rigidbody>();
            transformRefs.gameObject.layer = LayerMask.NameToLayer(ChangeThemToLayer);
            transformRefs.GetComponent<Dissolve>().PlayDissolveAnimation(0.0f, 3.0f, transformRefs.gameObject);
        }

        Destroy(this);
    }
}
