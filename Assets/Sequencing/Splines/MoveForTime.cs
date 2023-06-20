using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForTime : EnemyControlState {
    [SerializeField] float moveTime = 1f;
    [SerializeField] float currentTime = 0f;
    [SerializeField] bool shouldShoot = true;
    public override void EnterState(Enemy enemy)
    {
        currentTime = 0f;
    }

    public override bool UpdateEnemy(Enemy enemy)
    {
        enemy.transform.position -= enemy.transform.up * enemy.moveSpeed * Time.deltaTime;
        currentTime += Time.deltaTime;
        if (shouldShoot) enemy.TryToShoot();
        return currentTime >= moveTime;
    }
}
