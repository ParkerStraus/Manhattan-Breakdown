using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ThrowableObj : IForceObject
{
    [SerializeField] private float Damage;
    public AudioClip[] SFX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PrepareThrowable(float Damage, Vector2 throwSpeed, float SlowDownSpeed, float InertTime)
    {
        ForceInterp = SlowDownSpeed;
        SetInertia(InertTime);
        this.Damage = Damage;
        ApplyForce(throwSpeed);

    }


    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
