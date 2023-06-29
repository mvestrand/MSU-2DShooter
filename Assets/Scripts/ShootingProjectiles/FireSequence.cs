using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

public struct FireSequenceState {
    public float t;
    public int index;

    public void Stop() {
        t = 0;
        index = -1;
    }

    public void Start() {
        t = 0;
        index = 0;
    }
}

[System.Serializable]
public enum FireSequenceFrameMode {
    NoProjectile,
    Projectile,
    Pattern,
}

[System.Serializable]
public struct FireSequenceFrame {
    [Min(0)]public float time;
    public Projectile projectile;
    public BulletPattern pattern;
    public GameObject effect;
    public FireSequenceFrameMode mode;

}

[CreateAssetMenu(menuName = "Fire Sequence")]
public class FireSequence : ScriptableObject {

    [SerializeField] private float endTime;
    [SerializeField] private List<FireSequenceFrame> frames = new List<FireSequenceFrame>();

    /// <summary>
    /// Updates the fire sequence state for a given ShootingController
    /// </summary>
    /// <param name="controller">The ShootingController to use </param>
    /// <param name="deltaTime">The amount of time in seconds to advance the sequence</param>
    /// <param name="state">The controller's instance state to use and update.</param>
    /// <param name="tryingToFire">Is the controlling entity trying to keep firing?</param>
    /// <returns>True if the fire sequence is still running, false otherwise</returns>
    public bool Execute(ShootingController controller, ref FireSequenceState state, float deltaTime, bool tryingToFire) {
        if (state.index < 0) {
            if (!tryingToFire)
                return false;
            else
                state.Start();
        }
        while (deltaTime > 0) {
            if (state.index >= frames.Count || frames[state.index].time > endTime) {  // No more frames, go until end time is hit
                state.t += deltaTime;
                deltaTime = state.t - endTime;
                if (deltaTime >= 0) { // Loop back to beginning if still trying to fire, otherwise finish sequence
                    if (tryingToFire) {
                        state.Start();
                    } else {
                        state.Stop();
                        return false;
                    }
                } else
                    return true;

            } else if (state.t + deltaTime >= frames[state.index].time) { // Enough time has passed to execute the next frame
                deltaTime = deltaTime - (frames[state.index].time - state.t);
                ExecuteFrame(controller, ref state, deltaTime, tryingToFire);

            } else {  // Wait for next frame
                state.t += deltaTime;
                return true;
            }
        }

        return true;
    }

    public void OnValidate() {
        SortFrames();
    }

    private void SortFrames() {
        frames.Sort((f1, f2) => f1.time.CompareTo(f2.time));
    }

    private void ExecuteFrame(ShootingController controller, ref FireSequenceState state, float deltaTime, bool tryingToFire) {
        // Spawn projectiles specified by the frame
        if (frames[state.index].mode == FireSequenceFrameMode.Projectile)
            controller.SpawnSingleProjectile(frames[state.index].projectile?.gameObject, deltaTime);
        else if (frames[state.index].mode == FireSequenceFrameMode.Pattern)
            controller.SpawnPattern(frames[state.index].pattern, deltaTime);
        
        if (frames[state.index].effect != null) { // Create effect if any is needed
            Pool.Instantiate(frames[state.index].effect, controller.transform.position, controller.transform.rotation);
        }

        // Update the state
        state.t = frames[state.index].time;
        state.index++;
    }

}
