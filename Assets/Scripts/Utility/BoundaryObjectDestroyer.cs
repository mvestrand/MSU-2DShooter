using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

/// <summary>
/// A class which destroys it's gameobject after a certain amount of time
/// </summary>
public class BoundaryObjectDestroyer : MonoBehaviour
{
    public Vector2 minBound = new Vector2 (-10,-12);
    public Vector2 maxBound = new Vector2 (10,12);

    

    [Tooltip("Whether to destroy child gameobjects when this gameobject is destroyed")]
    public bool destroyChildrenOnDeath = true;

    // Flag which tells whether the application is shutting down (helps avoid errors)
    public static bool quitting = false;

    /// <summary>
    /// Description:
    /// Standard Unity function called when the application quits
    /// 
    /// Ensures that the quitting flag gets set correctly to avoid work as the application quits
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    // private void OnApplicationQuit()
    // {
    //     // quitting = true;
    //     // DestroyImmediate(this.gameObject);
    // }



    /// <summary>
    /// Description:
    /// Every frame, increment the amount of time that this gameobject has been alive,
    /// or if it has exceeded it's maximum lifetime, destroy it
    /// Inputs: none
    /// Returns: void (no return)
    /// </summary>
    void Update()
    {
        if (transform.position.x > maxBound.x || transform.position.y > maxBound.y || 
            transform.position.x < minBound.x || transform.position.y < minBound.y) {
            if (TryGetComponent<PooledMonoBehaviour>(out var pooling)) {
                pooling.Release();
            } else {
                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    /// Description:
    /// Behavior which triggers when this component is destroyed
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    // private void OnDestroy()
    // {
    //     // if (destroyChildrenOnDeath && !quitting && Application.isPlaying)
    //     // {
    //     //     int childCount = transform.childCount;
    //     //     for (int i = childCount - 1; i >= 0; i--)
    //     //     {
    //     //         GameObject childObject = transform.GetChild(i).gameObject;
    //     //         if (childObject != null)
    //     //         {
    //     //             DestroyImmediate(childObject);
    //     //         }
    //     //     }
    //     // }
    //     // transform.DetachChildren();
    // }
}
