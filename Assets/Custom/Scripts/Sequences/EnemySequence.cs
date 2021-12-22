using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
public class EnemySequence : MonoBehaviour, ISequence {


    private bool _allEnemiesDefeated;

    [AssetsOnly][SerializeField] private List<Enemy> enemyPrefabs = new List<Enemy>();
    [ShowInInspector] private List<Enemy> enemyInstances = new List<Enemy>();

    private bool _hasStarted;
    public bool WasPlayed { get { return _hasStarted; } }

    public bool Block { 
        get {
            return false;
        }
    }


    public bool Finished => throw new System.NotImplementedException();

    public bool NeedsCleanup => throw new System.NotImplementedException();

    public void CheckFlag(ISequenceFlag flag, out FlagStatus status)
    {
        throw new System.NotImplementedException();
    }

    public void Cleanup()
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public void EndEarly()
    {
        throw new System.NotImplementedException();
    }

    public void Play()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
