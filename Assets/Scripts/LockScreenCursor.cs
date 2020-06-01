using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScreenCursor : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LockScreenCursorCoroutine());
    }

    private IEnumerator LockScreenCursorCoroutine()
    {
        for (int i = 0; i < 8; i++)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            yield return new WaitForSeconds(1.0f);
        }

        yield return null;
    }
}
