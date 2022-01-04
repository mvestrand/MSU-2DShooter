using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

[CreateAssetMenu(menuName ="Custom/Enemy Type")]
public class EnemyType : ScriptableObject {


    public float moveSpeed = 5;
    public float turnSpeed = 10000;

    public float followRange = 10;

    public int scoreValue = 5;

    public StateMachineTemplate<Enemy> stateMachine = null;
    public StateBase<Enemy> entryState = null;
}
