using UnityEngine;
using UnityEngine.Playables;

public class AnimationTriggerReceiver : MonoBehaviour, INotificationReceiver
{
    [SerializeField] Animator target;

    public void OnNotify(Playable origin, INotification notification, object context) {
        var animationTriggerMarker = notification as AnimationTriggerMarker;
        if (animationTriggerMarker == null || target == null)
            return;
        target.SetTrigger(animationTriggerMarker.TriggerName);
    }
}
