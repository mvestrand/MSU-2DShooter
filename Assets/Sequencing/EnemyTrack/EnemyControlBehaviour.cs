using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using MVest.Unity.Pooling;

[System.Serializable]
public class EnemyControlBehaviour : PlayableBehaviour {



    [HideInInspector] public Enemy instance;
    [HideInInspector] public BezierSpline spline;

    public float t;
    public float direction;
    [Range(0, 1)] public float moveDirectionWeight;
    public bool shouldTrackPlayer;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        if (instance == null)
            return;
        Vector3 desiredPosition = instance.transform.position;
        float desiredRotation = 0;

        desiredRotation += Vector3.SignedAngle(Vector3.down, spline.GetVelocity(t).normalized, Vector3.forward) * moveDirectionWeight;
        desiredRotation += (1 - moveDirectionWeight) * direction;

        instance.transform.position = spline.GetPoint(t);
        instance.transform.eulerAngles = new Vector3(0, 0, desiredRotation);
    }

    public override void OnPlayableDestroy(Playable playable) {
        if (instance != null) {
            GameObject.Destroy(instance);
            instance = null;
        }
        spline = null;
    }

    //     public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
    //         if (instance == null && prefab != null)
    //             instance = Pool.Instantiate(prefab.gameObject).GetComponent<Enemy>();
    //         if (instance == null)
    //             return;
    // //        instance.transform.position = 
    //     }

    //     public override void OnGraphStart(Playable playable)
    //     {
    //         base.OnGraphStart(playable);
    //     }

    //     public override void OnGraphStop(Playable playable)
    //     {
    //         base.OnGraphStop(playable);
    //     }
}
