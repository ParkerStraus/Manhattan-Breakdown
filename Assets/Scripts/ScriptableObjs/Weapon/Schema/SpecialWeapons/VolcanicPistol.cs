using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/VolcanicPistol")]
public class VolcanicPistol : Gun
{
    public int SoundRack;
    public int SoundBreak;
    public override int UseWeapon(UnityEngine.Transform attackPoint, GameObject player)
    {

        if (NextBulletReady = false && AttackTimer >= AttackRate)
        {
            NextBulletReady = true;
            OnNextBulletReady.Invoke();
        }
        //Debug.Log("Gun");
        if (CurrentAmmo > 0)
        {
            if (CurrentAmmo % 2 == 0)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (AttackTimer >= AttackRate)
                    {
                        //Random chance to break
                        if(Random.Range(0f, 1f) > 0.85f)
                        {
                            BreakGun();
                        }

                        AttackTimer = 0;
                        //Debug.Log("Bagoom");
                        GunShot(attackPoint, player);
                        Player.localInstance.SendWeaponInfo();
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

    public new void GunShot(UnityEngine.Transform attackPoint, GameObject player)
    {
        if (CurrentAmmo > 0)
        {
            CurrentAmmo--;
            PlayerAudio.localInstance.PlaySound(audio_gunshot, 0.2f);
            GameObject.Find("VirCam").GetComponent<VirCamStuff>().Shake(0.9f, 1.5f, 0.2f, 0f);
            //Debug.Log(attackPoint.rotation.eulerAngles.z);
            Debug.DrawRay(attackPoint.position,
                Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z) * Vector2.right, Color.white, 0.1f);
            //Shoot Ray
            RaycastHit2D[] hits = Physics2D.RaycastAll(attackPoint.position,
                Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z) * Vector2.right, 9999, ~(LayerMask.GetMask("Items")));
            bool Hitted = false;
            foreach (RaycastHit2D hit in hits)
            {

                Instantiate(Impact, hit.point, Quaternion.LookRotation(hit.normal, Vector3.left));
                IDamageable obj = hit.collider.gameObject.GetComponent<IDamageable>();
                if (obj != null && obj.GetTangible())
                {
                    if (obj.Damage(damage))
                    {
                        KillConfirm(player);
                    }
                    //CreateParticle(obj.GetImpactEffect(), hit);
                    obj.CreateImpact(hit);
                }

                EffectableObject e = hit.collider.gameObject.GetComponent<EffectableObject>();
                if (e != null)
                {
                    e.InjectBurning(5.5f);
                }

                EffectsManager.instance.Tracer(hit, attackPoint);
                Hitted = true;
                break;
            }
            if (!Hitted)
            {
                EffectsManager.instance.Tracer(Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z) * Vector3.right * 20 + attackPoint.position, attackPoint);
            }
            if (CurrentAmmo == 0)
            {
                OnLastBulletFire();
            }
        }
    }

    private void BreakGun()
    {
        Debug.Log("Gun broke now");
        Player.localInstance.DeleteGun();
    }

    //Even: Ready to shoot
    //Odd: Ready to rack
    public override string GetAmmoString()
    {
        return (CurrentAmmo / 2).ToString();
    }
}
