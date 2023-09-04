using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SetDirectorTimeOnLoad : MonoBehaviour
{
    [SerializeField] PlayableDirector director;

    protected void Start() {
        if (LevelLoadManager.StartTimeSet) {
            if (director.state == PlayState.Playing) {
                director.Pause();
                director.time = LevelLoadManager.StartTime;
                director.Resume();
            } else {
                director.initialTime = LevelLoadManager.StartTime;
            }
        }
    }
}
