using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable {
    int TeamId { get; }
    void HandleDamage(Damage damage);
}
