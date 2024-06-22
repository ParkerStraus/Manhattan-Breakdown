using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class Garand : Gun
{
    public int Audio_Ping;
    public GameObject ClipOBJ;

    public override int UseWeapon(UnityEngine.Transform attackPoint, GameObject player)
    {

        if(NextBulletReady = false && AttackTimer >= AttackRate)
        {
            NextBulletReady = true;
            OnNextBulletReady.Invoke();
        }
        //Debug.Log("Gun");
                    //Debug.Log("Now accessing Semi Auto guns");
                    if (Input.GetButtonDown("Fire1") && CurrentAmmo > 0)
        {
                        if (AttackTimer >= AttackRate)
                        {
                            if (CurrentAmmo == 1) GarandPing(attackPoint);
                            AttackTimer = 0;
                            //Debug.Log("Bang");
                            GunShot(attackPoint, player);
                            player.GetComponent<Player>().SendWeaponInfo();
                            NextBulletReady = false;
                        }
                    }
        else
        {
            if (Input.GetButtonDown("Fire1")) PlayerAudio.localInstance.PlaySound(audio_click);
        }

        AttackTimer += Time.deltaTime;
        return 0;
    }

    public void GarandPing(UnityEngine.Transform attackPoint)
    {
        PlayerAudio.localInstance.PlaySound_Disconnected(Audio_Ping);
    }


    public override string GetAmmoString()
    {
        return CurrentAmmo.ToString();
    }


}
