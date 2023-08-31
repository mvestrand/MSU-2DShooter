using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;



public class AnimationTriggerMarker : MonoBehaviour, INotification, INotificationOptionProvider
{
    public PropertyName id { get; }
    [SerializeField] string triggerName;
    public string TriggerName { get { return triggerName; } }

    NotificationFlags INotificationOptionProvider.flags => NotificationFlags.Retroactive;
}