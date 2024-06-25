using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/CountingPistol")]
public class CountingGun : Gun
{
    public int[] SoundCounting;
    public bool Lock = false;
    public float CountBurstInterval;
    public float CountBurstTime;
    public int TargetAmt;
    public int TargetCount;
    public override int UseWeapon(UnityEngine.Transform attackPoint, GameObject player)
    {
        if (Lock) ContinueBurst(attackPoint, player);
        //Debug.Log("Gun");
        if (CurrentAmmo > 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (AttackTimer >= AttackRate)
                {
                    Lock = true;
                    AttackTimer = 0;
                    //Debug.Log("Bagoom");
                    CountingBurst(attackPoint, player);
                    Player.localInstance.SendWeaponInfo();
                }
            }


        }
        else
        {
            if (Input.GetButtonDown("Fire1")) PlayerAudio.localInstance.PlaySoundClient(audio_click);
        }

        AttackTimer += Time.deltaTime;
        return 0;
    }

    public void CountingBurst(UnityEngine.Transform attackPoint, GameObject player)
    {
        CurrentAmmo--;
        TargetCount = 0;
        if(CurrentAmmo == 4)
        {
            PlaySound(SoundCounting[0]);
            GunShot(attackPoint, player);
        }
        TargetAmt = 4 - CurrentAmmo;
        PlayerAudio.localInstance.PlaySound_Disconnected(SoundCounting[4 - CurrentAmmo]);
        GunShot(attackPoint, player);
        CountBurstTime = 0;
        Lock = true;
    }
    public void ContinueBurst(UnityEngine.Transform attackPoint, GameObject player)
    {
        if (TargetCount >= TargetAmt)
        {
            Lock = false;
            return;
        }
        CountBurstTime += Time.deltaTime;
        if (CountBurstTime >= CountBurstInterval)
        {
            CountBurstTime -= CountBurstInterval;
            TargetCount++;
            GunShot(attackPoint, player);
        }

    }

    public new void GunShot(UnityEngine.Transform attackPoint, GameObject player)
    {
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
                EffectsManager.instance.Tracer(hit, attackPoint);
                Hitted = true;
                break;
            }
            if (!Hitted)
            {
                EffectsManager.instance.Tracer(Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z) * Vector3.right * 20 + attackPoint.position, attackPoint);
            }
    }
}
