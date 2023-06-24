using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyControlMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var enemy = playerData as Enemy;
        if (enemy == null)
            return;

        // Vector3 desiredPosition = enemy.transform.position;
        // float desiredRotation = 0;

        // desiredRotation += Vector3.SignedAngle(Vector3.down, spline.GetVelocity(t).normalized, Vector3.forward) * moveDirectionWeight;
        // desiredRotation += (1 - moveDirectionWeight) * direction;

        // enemy.transform.position = spline.GetPoint(t);
        // enemy.transform.eulerAngles = new Vector3(0, 0, desiredRotation);

    }
}
