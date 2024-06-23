using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public interface IInGameEffects : IDamageable
{
    float Time_Burning
    {
        get; set;
    }

    float Time_Blinded
    {
        get; set;
    }

    void InjectBurning(float Time)
    {
        if(Time > Time_Burning)
        {
            Time_Burning = Time;
        }
    }

}
