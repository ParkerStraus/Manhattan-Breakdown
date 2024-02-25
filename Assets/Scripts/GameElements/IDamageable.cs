using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IDamageable
{

    bool Damage(float damage)
    {
        return false;
    }

    GameObject GetImpactEffect()
    {
        return null;
    }

}


