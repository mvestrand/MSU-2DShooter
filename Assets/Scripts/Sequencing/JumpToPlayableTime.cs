using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class JumpToPlayableTime : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private List<double> destinationTimes = new List<double>();

    public void JumpToTime(int index) {
        if (index < 0 || index >= destinationTimes.Count) {
            return;
        }

        double destTime = destinationTimes[index];
        director.time = destTime;
    }

}
