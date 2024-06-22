using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/PumpShotgun")]
public class PumpShotgun : Gun
{
    public int SoundRack;
    public override int UseWeapon(UnityEngine.Transform attackPoint,  GameObject player)
    {

        if (NextBulletReady = false && AttackTimer >= AttackRate)
        {
            NextBulletReady = true;
            OnNextBulletReady.Invoke();
        }
        //Debug.Log("Gun");
        if (CurrentAmmo > 0)
        {
            if(CurrentAmmo % 2 == 0)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (AttackTimer >= AttackRate)
                    {
                        AttackTimer = 0;
                        //Debug.Log("Bagoom");
                        GunShot_Scatter(attackPoint, player);
                        player.GetComponent<Player>().SendWeaponInfo();
                    }
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (AttackTimer >= AttackRate)
                    {
                        AttackTimer = 0;
                        CurrentAmmo--;
                        PlayerAudio.localInstance.PlaySound(SoundRack);
                    }
                }
                //Rack shotgun
            }
                    
        }
        else
        {
            if (Input.GetButtonDown("Fire1")) PlayerAudio.localInstance.PlaySoundClient(audio_click);
        }

        AttackTimer += Time.deltaTime;
        return 0;
    }

    //Even: Ready to shoot
    //Odd: Ready to rack
    public override string GetAmmoString()
    {
        return (CurrentAmmo / 2).ToString();
    }
}
