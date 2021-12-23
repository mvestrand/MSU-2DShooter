using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
public class EnemySequence : TimedSequence {

    [Header("Enemy Settings")]
    [AssetsOnly][SerializeField] private List<Enemy> enemyPrefabs = new List<Enemy>();
    [SerializeField] private List<EnemyController> controllers = new List<EnemyController>();

    [HorizontalGroup("OnAllDead", Title = "On All Enemies Dead", LabelWidth = 5)]
    [LabelText("Unblock")][ToggleLeft]
    [SerializeField] private bool unblockOnAllDead = true;
    [HorizontalGroup("OnAllDead")]
    [LabelText("Finish")][ToggleLeft]
    [SerializeField] private bool finishOnAllDead = true;
    [HorizontalGroup("OnAllDead")]
    [LabelText("Cleanup")][ToggleLeft]
    [SerializeField] private bool cleanupOnAllDead = true;

    [ReadOnly][ShowInInspector] private List<Enemy> enemyInstances = new List<Enemy>();

    private bool _cleared = false;
    public bool Cleared { get { return _cleared; } }
    private int _enemiesDefeated = 0;
    public int EnemiesDefeated { get { return _enemiesDefeated; } }
    public int TotalEnemies { get { return enemyPrefabs.Count; } }

    public override void Play() {
        base.Play();

        _enemiesDefeated = 0;
        _cleared = false;

        for (int i = 0; i < enemyPrefabs.Count; i++) {
            Enemy newEnemy = enemyPrefabs[i].Get<Enemy>(controllers[i].Position, controllers[i].Rotation);
            enemyInstances.Add(newEnemy);
            newEnemy.onDespawn += EnemyDespawned;
            newEnemy.onDeath += EnemyDestroyed;
            newEnemy.controller = controllers[i];
        }
    }

    public override void Finish() {
        base.Finish();
    }

    public override void Cleanup() {
        foreach (var enemy in enemyInstances) {
            enemy.onDeath -= EnemyDestroyed;
            enemy.onDespawn -= EnemyDespawned;
            enemy.Release();
        }
        enemyInstances.Clear();
        base.Cleanup();
    }

    public override void Clear()
    {
        base.Clear();
        _enemiesDefeated = 0;
        _cleared = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void EnemyDestroyed(Enemy enemy) {
        enemyInstances.Remove(enemy);
        enemy.onDeath -= EnemyDestroyed;
        enemy.onDespawn -= EnemyDespawned;
        enemy.controller = null;
        _enemiesDefeated++;
        if (enemyInstances.Count == 0) {
            _cleared = true;
            NoMoreEnemiesRemaining();
        }
    }

    private void EnemyDespawned(Enemy enemy) {
        enemyInstances.Remove(enemy);
        enemy.onDeath -= EnemyDestroyed;
        enemy.onDespawn -= EnemyDespawned;
        enemy.controller = null;
        if (enemyInstances.Count == 0) {
            NoMoreEnemiesRemaining();
        }
    }

    private void NoMoreEnemiesRemaining() {
        if (unblockOnAllDead)
            this.AllowUnblock();
        if (cleanupOnAllDead)
            this.AllowCleanup();
        else if (finishOnAllDead)
            this.AllowFinish();
    }

    public void AllowBoundingBoxDespawns() {
        foreach (var controller in controllers) {
            controller.canDespawn = true;
        }
    }

}
