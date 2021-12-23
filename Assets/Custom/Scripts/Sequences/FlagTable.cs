using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlagTable : ScriptableObject
{


    private ISequenceFlag _allEnemiesDefeated;
    public ISequenceFlag AllEnemiesDefeated { get { return _allEnemiesDefeated; } }
}
