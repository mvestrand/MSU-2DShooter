using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MVest.Unity.Pooling;

public abstract class EnemyControlState : MonoBehaviour {
    public abstract void EnterState(Enemy enemy);
    public abstract bool UpdateEnemy(Enemy enemy);
}

[CreateAssetMenu(menuName ="EnemyControlClip")]
public class EnemyControlClip : ScriptableObject {
    [SerializeField] List<EnemyControlState> controlStates = new List<EnemyControlState>();
    public void UpdateEnemy(Enemy enemy) {
        if (enemy.clipIndex >= controlStates.Count) {
            Destroy(enemy);
            return;
        }
        if (enemy.state == null) {
            enemy.state = Instantiate<EnemyControlState>(controlStates[enemy.clipIndex], enemy.transform);
            enemy.state.EnterState(enemy);
        }
        bool shouldAdvance = enemy.state.UpdateEnemy(enemy);
        if (shouldAdvance) {
            enemy.clipIndex++;
            if (enemy.clipIndex >= controlStates.Count) {
                Destroy(enemy);
                return;
            }
            Destroy(enemy.state);
            enemy.state = Instantiate<EnemyControlState>(controlStates[enemy.clipIndex], enemy.transform);
            enemy.state.EnterState(enemy);
        }
    }
}
