using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Globals;

public class WasDestroyedBool : MonoBehaviour
{
    [SerializeField] private GlobalBool target;

    // Start is called before the first frame update
    protected void OnEnable()
    {
        if (target != null) {
            target.Value = false;
        }
    }

    public void SetTrueDelayed(float delay) {
        if (target == null)
            return;
        StartCoroutine(SetTrueAfterDelay(delay));
    }

    public void SetValue(bool value) {
        target.Value = value;
    }

    private IEnumerator SetTrueAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        target.Value = true;
    }

}
