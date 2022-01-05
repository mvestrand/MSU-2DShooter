using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

[CreateUniqueAsset("Custom/Data/Enemy/FSM/States")]
public class FlyForwardState : State<EnemyFSM> {


    public override void OnEnter(EnemyFSM sharedData) {}

    public override void OnExit(EnemyFSM sharedData){}

    public override void Tick(EnemyFSM enemy) {
        enemy.transform.position += enemy.enemyType.moveSpeed * -enemy.transform.up * Time.deltaTime;
        //enemy.
    }
}
