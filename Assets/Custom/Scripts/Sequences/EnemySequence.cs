using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[RequireComponent(typeof(BoundingBox))]
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

    private bool _sequenceCompleted = false;
    public bool Cleared { get { return _sequenceCompleted; } }
    private int _enemiesAvailable = 0;
    private int _enemiesDefeated = 0;
    public int EnemiesDefeated { get { return _enemiesDefeated; } }
    public int TotalEnemies { get { return enemyPrefabs.Count; } }

    private void Awake() {
        BoundingBox despawnBox = GetComponent<BoundingBox>();
        foreach (var controller in controllers) {
            controller.despawnBox = despawnBox;
        }
    }

    public void Spawn(int prefabIndex, EnemyController controller) {
        Enemy newEnemy = enemyPrefabs[prefabIndex].Get<Enemy>(controller.Position, controller.Rotation);
        AttachEnemy(newEnemy, controller);
    }

    public override void Play() {
        base.Play();

        _enemiesDefeated = 0;
        _sequenceCompleted = false;
        _enemiesAvailable = controllers.Count;

        foreach (var controller in controllers) {
            controller.owningSequence = this;
        }
    }

    public override void Finish() {
        base.Finish();
    }

    public override void Cleanup() {
        foreach (var enemy in enemyInstances) {
            enemy.onDeath -= EnemyDestroyed;
            enemy.onDespawn -= EnemyDespawned;
            enemy.DetachController();
            enemy.Release();
        }
        enemyInstances.Clear();
        base.Cleanup();
    }

    public override void Clear()
    {
        base.Clear();
        _enemiesDefeated = 0;
        _sequenceCompleted = false;
        _enemiesAvailable = controllers.Count;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void EnemyDestroyed(Enemy enemy) {
        DetachEnemy(enemy);
        _enemiesDefeated++;
        if (_enemiesAvailable <= 0) {
            _sequenceCompleted = true;
            NoMoreEnemiesRemaining();
        }
    }

    private void AttachEnemy(Enemy enemy, EnemyController controller) {
            enemyInstances.Add(enemy);
            enemy.onDespawn += EnemyDespawned;
            enemy.onDeath += EnemyDestroyed;
            enemy.Attach(controller);
    }

    public void DetachEnemy(Enemy enemy) {
        enemyInstances.Remove(enemy);
        enemy.onDeath -= EnemyDestroyed;
        enemy.onDespawn -= EnemyDespawned;
        _enemiesAvailable--;
        enemy.DetachController();
    }

    private void EnemyDespawned(Enemy enemy) {
        DetachEnemy(enemy);
        if (_enemiesAvailable <= 0) {
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



    private static int controlNo;
    [ContextMenu("Grab Controllers")]
    private void GrabControllers() {
        controlNo = 0;
        controllers.Clear();
        foreach (var controller in GetComponentsInChildren<EnemyController>()) {
            controller.gameObject.name = string.Format("E{0:D3}", controlNo);
            controllers.Add(controller);
            controlNo++;
        }
    }



}
