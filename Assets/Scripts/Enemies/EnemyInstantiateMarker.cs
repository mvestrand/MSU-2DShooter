using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;



[DisplayName("Enemy/Instantiate")]
public class EnemyInstantiateMarker : Marker, INotification, INotificationOptionProvider
{
    [SerializeField] public bool emitOnce;
    [SerializeField] public bool emitInEditor;
    public PropertyName id { get; }

    NotificationFlags INotificationOptionProvider.flags =>
        (emitOnce ? NotificationFlags.TriggerOnce : default) |
        (emitInEditor ? NotificationFlags.TriggerInEditMode : default);
}
