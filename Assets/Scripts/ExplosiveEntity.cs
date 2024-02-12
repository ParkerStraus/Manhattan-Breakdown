using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveEntity : MonoBehaviour, IDamageable
{
    [SerializeField] private float Health;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Damage(float damage)
    {
        Health -= damage;
        if(Health <= 0 ) OnExplosion(); 
        return false;
    }

    void OnExplosion()
    {
        Debug.Log("Kaboosh");
        Destroy(this.gameObject);
    }
}
