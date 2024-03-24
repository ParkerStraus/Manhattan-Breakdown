using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Gun")]
public class Gun : Weapon
{

    public enum FireMode
    {
        SEMI,
        BURST,
        FULL,
        SCATTER
    }

    public FireMode _firemode;

    [Header("Gun Info")]
    public int MaxAmmo;
    public int CurrentAmmo;
    public float ConeRadius;
    public int BurstAmount;
    public int BurstCurrent = 0;
    public float BurstInterval;
    public int ConeRayAmount;
    public UnityEvent OnLastBulletFire_Event;
    public bool NextBulletReady;
    public UnityEvent OnNextBulletReady;

    public override void Initialize()
    {
        CurrentAmmo = MaxAmmo;
        AttackTimer = AttackRate;
        em = GameObject.Find("Main Camera").GetComponent<EffectsManager>();
    }

    public override int UseWeapon(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        this.ac = ac;
        if(NextBulletReady = false && AttackTimer >= AttackRate)
        {
            NextBulletReady = true;
            OnNextBulletReady.Invoke();
        }
        Debug.Log("Gun");
        if (CurrentAmmo > 0)
        {
            switch (_firemode)
            {
                case FireMode.SEMI:
                    //Debug.Log("Now accessing Semi Auto guns");
                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (AttackTimer >= AttackRate)
                        {
                            AttackTimer = 0;
                            //Debug.Log("Bang");
                            GunShot(attackPoint, ac, player);
                            player.GetComponent<Player>().SendWeaponInfo();
                            NextBulletReady = false;
                        }
                    }
                    break;

                case FireMode.FULL:
                    if (Input.GetButton("Fire1"))
                    {
                        if (AttackTimer >= AttackRate)
                        {
                            AttackTimer = 0;
                            //Debug.Log("Tacka");
                            GunShot(attackPoint, ac, player);
                            player.GetComponent<Player>().SendWeaponInfo();
                        }

                    }
                    break;

                case FireMode.BURST:
                    if (Input.GetButton("Fire1"))
                    {
                        if (AttackTimer >= AttackRate)
                        {
                            if (BurstCurrent < BurstAmount)
                            {
                                BurstCurrent++;
                                AttackTimer = 0;
                                //Debug.Log("RadaTada");
                                GunShot(attackPoint, ac, player);
                                player.GetComponent<Player>().SendWeaponInfo();
                            }
                        }
                    }
                    else if (Input.GetButtonUp("Fire1"))
                    {
                        BurstCurrent = 0;
                    }
                    break;

                case FireMode.SCATTER:
                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (AttackTimer >= AttackRate)
                        {
                            AttackTimer = 0;
                            //Debug.Log("Bagoom");
                            GunShot_Scatter(attackPoint, ac, player);
                            player.GetComponent<Player>().SendWeaponInfo();
                        }
                    }
                    break;
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1")) ac.PlaySound(audio_click);
        }

        AttackTimer += Time.deltaTime;
        return 0;
    }
    public void GunShot(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        if (CurrentAmmo > 0)
        {
            CurrentAmmo--;
            ac.PlaySound(audio_gunshot, 0.2f);
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
                if (obj != null)
                {
                    if (obj.Damage(damage))
                    {
                        KillConfirm(player);
                    }
                    CreateParticle(obj.GetImpactEffect(), hit);
                }
                Tracer(hit, attackPoint);
                Hitted = true;
                break;
            }
            if(!Hitted)
            {
                Tracer(Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z) * Vector3.right * 20 + attackPoint.position, attackPoint);
            }
            if (CurrentAmmo == 0)
            {
                OnLastBulletFire();
            }
        }
    }

    public void GunShot_Scatter(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        if (CurrentAmmo > 0)
        {
            CurrentAmmo--;
            ac.PlaySound(audio_gunshot, 0.2f);
            GameObject.Find("VirCam").GetComponent<VirCamStuff>().Shake(0.9f, 1.5f, 0.2f, 0f);
            //Debug.Log(attackPoint.rotation.eulerAngles.z);
            Debug.DrawRay(attackPoint.position,
                Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z) * Vector2.right, Color.white, 0.1f);
            //Shoot Ray
            for (int i = 0; i < ConeRayAmount; i++)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(attackPoint.position,
                    Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z - ConeRadius / 2 + (((float)(i + 1) / ConeRayAmount) * ConeRadius)) * Vector2.right, 9999, ~(LayerMask.GetMask("Items")));

                foreach (RaycastHit2D hit in hits)
                {

                    Instantiate(Impact, hit.point, Quaternion.LookRotation(hit.normal, Vector3.left));
                    IDamageable obj = hit.collider.gameObject.GetComponent<IDamageable>();
                    if (obj != null)
                    {
                        if (obj.Damage(damage / ConeRayAmount))
                        {
                            KillConfirm(player);
                        }
                        CreateParticle(obj.GetImpactEffect(), hit);
                    }
                    Tracer(hit, attackPoint);
                    break;
                }
            }
            if (CurrentAmmo == 0)
            {
                OnLastBulletFire();
            }
        }
    }

    public override string GetAmmo()
    {
        return CurrentAmmo.ToString();
    }

    public void OnLastBulletFire()
    {
        OnLastBulletFire_Event.Invoke();
    }
    public void Tracer(RaycastHit2D hit, UnityEngine.Transform attackPoint)
    {
        TrailRenderer TrailBase = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        TrailRenderer trail = Instantiate(TrailBase, attackPoint.position, Quaternion.identity);

        em.StartCoroutine(em.BulletTrailRoutine(trail, hit));
    }
    public void Tracer(Vector2 hit, UnityEngine.Transform attackPoint)
    {
        TrailRenderer TrailBase = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        TrailRenderer trail = Instantiate(TrailBase, attackPoint.position, Quaternion.identity);

        em.StartCoroutine(em.BulletTrailRoutine(trail, hit));
    }

}