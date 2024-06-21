using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class IForceObject : GameElement
{
    [SerializeField] protected Vector2 MoveForce;
    [SerializeField] protected float InertTime;
    [SerializeField] protected float ForceInterp;
    [SerializeField] protected Rigidbody2D rb;

    public void ApplyForce(Vector2 force)
    {
        //Debug.Log(force);
        MoveForce += force;
    }

    protected void Update()
    {
        //Debug.Log(MoveForce);
        if(InertTime > 0 || InertTime == -1)
        {
            InertTime -= Time.deltaTime;
        }
        else
        {
            MoveForce = Vector2.Lerp(MoveForce, Vector2.zero, ForceInterp * Time.deltaTime);
        }
    }

    protected void SetInertia(float inertia)
    {
        InertTime = inertia;
    }

    protected void FixedUpdate()
    {
        rb.velocity = (MoveForce);
    }
}
