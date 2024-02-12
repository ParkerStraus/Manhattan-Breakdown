using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour, IDamageable
{

    [SerializeField] private float Health = 100;
    [SerializeField] public int PID;
    public bool Damage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Destroy(this.gameObject);
            GameObject.Find("Main Camera").GetComponent<GameHandler>().OnKill(PID);
            return true;
        }
        return false;
    }
}
