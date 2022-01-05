using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;


[CreateUniqueAsset("Custom/Data/Enemy/FSM/States")]
public class ShootAtPlayer :  State<EnemyFSM> 
{
    public override void OnEnter(EnemyFSM enemy) {
        enemy.followTarget = GameManager.instance?.player?.transform;
    }
    public override void OnExit(EnemyFSM enemy) {}
    public override void Tick(EnemyFSM enemy) {
        if (enemy.followTarget != null && Vector3.Distance(enemy.transform.position, enemy.followTarget.position) <= enemy.enemyType.followRange) {

            // Turn towards target
            float targetAngle = enemy.GetAngle((enemy.followTarget.position - enemy.transform.position).normalized);
            float currentAngle = enemy.transform.eulerAngles.z;
            enemy.transform.eulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(currentAngle, targetAngle, Time.deltaTime * enemy.enemyType.turnSpeed));
        } 
        foreach (var gun in enemy.guns) {
            gun.FireHeld(true);
        }
    }
}
