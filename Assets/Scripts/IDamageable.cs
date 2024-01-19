using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool Damage(float damage)
    {
        return false;
    }
}
