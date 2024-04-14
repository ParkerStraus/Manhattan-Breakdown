using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Throwable")]
public class Throwable : Weapon
{
    public float ThrowSpeed;
    public float SlowDownSpeed;
    public float InertTime;
    public GameObject ThrowableObject;
    public override int UseWeapon(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        Debug.Log("Throwable");

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject obj = Instantiate(ThrowableObject, attackPoint);
            obj.GetComponent<ThrowableObj>().PrepareThrowable(damage, player.transform.right * ThrowSpeed, SlowDownSpeed, InertTime);
            obj.transform.parent = null;
            return -1;
        }
        return 0;
    }
    public override void Initialize()
    {
        em = GameObject.Find("Main Camera").GetComponent<EffectsManager>();
    }

    public override string GetAmmoString()
    {
        return "";
    }
}
