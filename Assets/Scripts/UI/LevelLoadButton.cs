using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class LevelLoadManager {

    public static bool StartTimeSet { get; private set; }
    private static double _startTime;
    public static double StartTime { get { return _startTime; } private set { _startTime = value; StartTimeSet = true; } }

    public static void LoadLevelAtTime(string levelName, double startTime) {
        ClearLevelLoadData();
        StartTime = startTime;
        SceneManager.LoadScene(levelName);
    }

    public static void LoadLevel(string levelName) {
        ClearLevelLoadData();
        SceneManager.LoadScene(levelName);
    }

    private static void ClearLevelLoadData() {
        StartTimeSet = false;
    }

}

/// <summary>
/// This class is meant to be used on buttons as a quick easy way to load levels (scenes)
/// </summary>
public class LevelLoadButton : MonoBehaviour
{

    /// <summary>
    /// Description:
    /// Loads a level according to the name provided
    /// Input:
    /// string levelToLoadName
    /// Returns:
    /// void (no return)
    /// </summary>
    /// <param name="levelToLoadName">The name of the level to load</param>
    public void LoadLevelByName(string levelToLoadName)
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        LevelLoadManager.LoadLevel(levelToLoadName);
    }

    public void LoadLevelByNameAtTime(string levelToLoadName, double time) {
        AudioListener.pause = false;
        Time.timeScale = 1;
        LevelLoadManager.LoadLevelAtTime(levelToLoadName, time);
    }

    public void LoadLevelByNameAtTime(LevelLoadPoint loadPoint) {
        AudioListener.pause = false;
        Time.timeScale = 1;
        LevelLoadManager.LoadLevelAtTime(loadPoint.LevelName, loadPoint.LevelInitTime);
    }
}
