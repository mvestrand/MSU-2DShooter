using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest;

[CreateUniqueAsset("Custom/Data/FSM/Enemy")]
public class FlyForwardState : State<Enemy> {


    public override void OnEnter(Enemy sharedData) {}

    public override void OnExit(Enemy sharedData){}

    public override void Tick(Enemy sharedData) {
        Debug.Log("Hello world!");
    }
}
