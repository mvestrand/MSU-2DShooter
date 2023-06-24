using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Timeline;
using UnityEngine.Playables;


[TrackColor(.8f, .3f, 0f)]
[TrackClipType(typeof(EnemyControlClip))]
[TrackBindingType(typeof(Enemy))]
public class EnemyControlTrack : TrackAsset
{
}
