using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

using MVest;

public class EnemyFSM : PooledMonoBehaviour {

    public event System.Action<EnemyFSM> onDeath;
    //public event System.Action<EnemyFSM> onDespawn;

    [Required][SerializeField] private SpawnController _spawnController;
    public EnemyType enemyType;
    private StateMachine<EnemyFSM> fsm;
    
    [Tooltip("The transform of the object that this enemy should follow.")]
    public Transform followTarget = null;

     [Tooltip("The enemy's gun components")]
    public List<Gun> guns = new List<Gun>();



    protected void Start() {
        fsm = enemyType.stateMachine.CreateStateMachine(this, enemyType.entryState);
    }

    protected void LateUpdate() {
        fsm.Tick(this);
        // // Handle despawn
        // if (controller != null && controller.ShouldDespawn(transform.position)) {
        //     onDespawn?.Invoke(this);
        //     this.Release();
        // }
    }
    public override void Restart() {
        fsm.SetState(enemyType.entryState, this);
    }
    
    /// <summary>
    /// Description:
    /// This is meant to be called before destroying the gameobject associated with this script
    /// It can not be replaced with OnDestroy() because of Unity's inability to distiguish between unloading a scene
    /// and destroying the gameobject from the Destroy function
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void DoBeforeDestroy()
    {
        AddToScore();
        IncrementEnemiesDefeated();
        onDeath?.Invoke(this);
    }

    /// <summary>
    /// Description:
    /// Adds to the game manager's score the score associated with this enemy if one exists
    /// Input:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    private void AddToScore()
    {
        if (GameManager.instance != null && !GameManager.instance.gameIsOver)
        {
            GameManager.AddScore(enemyType.scoreValue);
        }
    }

    /// <summary>
    /// Description:
    /// Increments the game manager's number of defeated enemies
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    private void IncrementEnemiesDefeated()
    {
        if (GameManager.instance != null && !GameManager.instance.gameIsOver)
        {
            GameManager.instance.IncrementEnemiesDefeated();
        }       
    }

    public float GetAngle(Vector3 target) {
        return Vector3.SignedAngle(Vector3.down, target, Vector3.forward);
    }

}
