using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;

using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class EnemyController : MonoBehaviour  {
    // public Enemy prefab;
    // public Enemy instance;
    // public Vector3 startPoint = Vector3.zero;

    // public void OnEnable() {
    //     Debug.Log("Enable");
    //     if (prefab == null)
    //         return;

    //     if (instance == null) {
    //         // var startPoint = GetComponent<BezierSpline>().GetPoint(0);
    //         instance = Instantiate(prefab, startPoint, Quaternion.identity);
    //     }

    //     var timeline = GetComponent<PlayableDirector>();
    //     var timelineAsset = (TimelineAsset)timeline.playableAsset;
    //     foreach (var binding in timelineAsset.outputs) {
    //         if (binding.sourceObject is EnemyControlTrack track) {
    //             timeline.SetGenericBinding(track, instance);
    //         }
    //     }

    // }

    // public void OnDisable() {
    //     Debug.Log("Disable");
    // }

}
