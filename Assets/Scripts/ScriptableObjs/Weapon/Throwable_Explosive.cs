using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Throwable Explosive")]
public class Throwable_Explosive : Throwable
{
    [Header("Grenade Info")]
    public bool Cooked;
    public float CookTimer;
    public float CookTimer_Current;
    public override int UseWeapon(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        Debug.Log("Throwable");

        bool Expended = Use_Grenade(attackPoint, ac, player);
                if (Expended)
                {
                    return -1;
                }
        AttackTimer += Time.deltaTime;
        return 0;
    }
    public override void Initialize()
    {
        CookTimer_Current = CookTimer;
        em = GameObject.Find("Main Camera").GetComponent<EffectsManager>();
    }

    public bool Use_Grenade(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        if (Cooked)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                GameObject gre = Instantiate(ThrowableObject, attackPoint);
                gre.GetComponent<GrenadeObj>().PrepareGrenade(damage, CookTimer_Current, player.transform.right * ThrowSpeed, SlowDownSpeed, InertTime);
                gre.transform.parent = null;
                return true;
            }
            CookTimer_Current -= Time.deltaTime;
            if (CookTimer_Current <= 0)
            {
                ac.PlaySound(audio_click, 0.2f);
                GameObject gre = Instantiate(ThrowableObject, attackPoint);
                gre.GetComponent<GrenadeObj>().PrepareGrenade(damage, CookTimer_Current, player.transform.right * ThrowSpeed, SlowDownSpeed, InertTime);
                gre.transform.parent = null;
                return true;
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Cooked = true;
                CookTimer_Current = CookTimer;
            }

        }
        return false;
    }

    public override string GetAmmo()
    {
        return "";
    }
}
