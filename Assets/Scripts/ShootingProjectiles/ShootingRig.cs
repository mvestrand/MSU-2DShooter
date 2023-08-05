using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AYellowpaper;

using MVest.Unity;

[System.Serializable]
public class TransformFrame {
    public Transform target;
    [System.NonSerialized] public FloatTransform frameStart;
    [System.NonSerialized] public FloatTransform frameEnd;

    public void PushFrame() {
        if (target == null) return;
        frameStart = frameEnd;
        frameEnd.SetLocal(target);        
    }

    public void Init() {
        if (target == null) return;
        frameEnd.SetLocal(target);
        frameStart = frameEnd;
    }

    public void Revert() {
        if (target == null) return;
        frameEnd.ApplyLocal(target);
    }

    public void ApplyInterp(float t) {
        if (target == null) return;
        FloatTransform.InterpolateLocal(target, frameStart, frameEnd, t);
    }

}

public class ShootingRig : MonoBehaviour, IShootingController
{
    public bool allowShoot = true;
    public List<InterfaceReference<IShootingController, MonoBehaviour>> guns = new List<InterfaceReference<IShootingController, MonoBehaviour>>();
    public List<TransformFrame> transforms = new List<TransformFrame>();

    protected void OnEnable() {
        foreach (var tf in transforms) {
            tf.Init();
        }
    }

    public void PlayOneShot() {
        foreach (var gun in guns) {
            gun.Value.PlayOneShot();
        }
    }

    public float TimeToNextEvent(bool shouldFire)
    {
        float nextEvent = float.MaxValue;
        foreach (var gun in guns) {
            nextEvent = Mathf.Min(nextEvent, gun.Value.TimeToNextEvent(shouldFire));
        }
        return nextEvent;
    }

    public bool UpdateFireState(bool shouldFire, float deltaTime, float extraTime, ref float turnSpeedLimit)
    {
        bool isDone = true;
        foreach (var gun in guns) {
            bool result = !gun.Value.UpdateFireState(shouldFire, deltaTime, extraTime, ref turnSpeedLimit);
            isDone = isDone && result;
        }
        return isDone;
    }

    public bool UpdateFireState(bool shouldFire, ref float turnSpeedLimit) {
        return UpdateForFrame(shouldFire, ref turnSpeedLimit);
    }

    private bool UpdateForFrame(bool shouldFire, ref float turnSpeedLimit) {
        if (Time.timeScale <= 0 || Time.deltaTime <= 0)
            return false;


        // Update the transform frames
        foreach (var tf in transforms) {
            tf.PushFrame();
        }

        if (!allowShoot)
            return false;


        float frameTime = Time.deltaTime;
        float remTime = frameTime;
        float deltaTime = Mathf.Max(TimeToNextEvent(shouldFire), 0);

        bool isDone = false;

        int i = 0;
        while (remTime >= deltaTime && i++ < 1000) {


            // Apply approximate inter-frame transforms
            float t = 1 - (remTime - deltaTime) / frameTime;
            foreach (var tf in transforms) {
                tf.ApplyInterp(t);
            }

            isDone = true;
            foreach (var gun in guns) {
                bool result = !gun.Value.UpdateFireState(shouldFire, deltaTime, remTime - deltaTime, ref turnSpeedLimit);
                isDone = isDone && result;
            }

            // Get the next event
            remTime -= deltaTime;
            deltaTime = Mathf.Max(TimeToNextEvent(shouldFire), 0);
        }

        // Revert all transforms to their correct value
        foreach (var tf in transforms) {
            tf.Revert();
        }

        // No more events, do final update
        if (remTime > 0) {
            isDone = true;
            foreach (var gun in guns) {
                bool result = !gun.Value.UpdateFireState(shouldFire, remTime, 0, ref turnSpeedLimit);
                isDone = isDone && result;
            }
        }

        return isDone;
    }
}
