using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Load Point")]
public class LevelLoadPoint : ScriptableObject
{
    [SerializeField] private string levelName;
    [SerializeField] private double levelInitTime;
    public string LevelName { get { return levelName; } }
    public double LevelInitTime { get { return levelInitTime; } }
}
