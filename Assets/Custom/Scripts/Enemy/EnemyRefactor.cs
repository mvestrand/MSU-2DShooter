using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

using MVest;

public class EnemyRefactor : PooledMonoBehaviour {

    [Required][SerializeField] private SpawnController _spawnController;


    protected void Update() {

    }
    protected override void Restart() {}

    
}

public class EnemyMovementStateData {

}

public class EnemyShootStateData {

}