using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum PoseType
{
    None,
    Pistol,
    Rifle,
    Melee,
    TwoMelee
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Weapon Object")]
public class Weapon : ScriptableObject
{
    public enum WeaponType
    {
        Melee,
        Gun,
        Consumeable,
        Etc
    }
    public WeaponType _weaponType;

    public PoseType _poseType;

    public enum FireMode
    {
        SEMI,
        BURST,
        FULL,
        SCATTER
    }

    public FireMode _firemode;

    public string Name;
    public float damage;
    public float AttackTimer;
    public float AttackRate;

    [Header("Gun Info")]
    public int MaxAmmo;
    public int CurrentAmmo;
    public float ConeRadius;
    public int BurstAmount;
    public int BurstCurrent = 0;
    public float BurstInterval;
    public int ConeRayAmount;
    public UnityEvent OnLastBulletFire_Event;

    [Header("Weapon Info")]
    public float AttackSize;

    [Header("Grenade Info")]
    public bool Cooked;
    public float CookTimer;
    public float CookTimer_Current;
    public GameObject GrenadeOBJ;


    [Header("Aesthetic stuff here")]
    [SerializeField] private Sprite floorImage;
    [SerializeField] private Sprite HeldImage;
    [SerializeField] private AudioClip audio_gunshot;
    [SerializeField] private AudioClip audio_click;
    [SerializeField] private AudioClip audio_impact;
    [SerializeField] private GameObject Impact;
    [SerializeField] private EffectsManager em;

    public void Initialize()
    {
        CookTimer_Current = CookTimer;
        CurrentAmmo = MaxAmmo;
        em = GameObject.Find("Main Camera").GetComponent<EffectsManager>();
    }

    public int UseWeapon(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        //Debug.Log(_firemode);
        //Swing Melee at enemies
        switch (_weaponType)
        {
            case WeaponType.Melee:
                //Swing the thing like a bat
                if (Input.GetButtonDown("Fire1"))
                {
                    if (AttackTimer >= AttackRate)
                    {
                        AttackTimer = 0;
                        Debug.Log("Swoop");
                        ac.PlaySound(audio_gunshot);
                        Melee(attackPoint, ac, player);
                    }
                }
                break;

            case WeaponType.Gun:
                if(CurrentAmmo > 0)
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
                
                break;

            case WeaponType.Consumeable:
                bool Expended = Use_Grenade(attackPoint, ac, player);
                if (Expended)
                {
                    return -1;
                }
                else
                {
                    break;
                }
        }
        AttackTimer += Time.deltaTime;
        return 0;
    }

    public void GunShot(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        if(CurrentAmmo > 0)
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
                break;
            }
            if(CurrentAmmo == 0)
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
            for(int i = 0; i < ConeRayAmount; i++)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(attackPoint.position,
                    Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z - ConeRadius/2 + (((float)(i+1)/ConeRayAmount)*ConeRadius)) * Vector2.right, 9999, ~(LayerMask.GetMask("Items")));

                foreach (RaycastHit2D hit in hits)
                {

                    Instantiate(Impact, hit.point, Quaternion.LookRotation(hit.normal, Vector3.left));
                    IDamageable obj = hit.collider.gameObject.GetComponent<IDamageable>();
                    if (obj != null)
                    {
                        if (obj.Damage(damage/ ConeRayAmount))
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

    public void OnLastBulletFire()
    {
        OnLastBulletFire_Event.Invoke();
    }

    public void Melee(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, ~(LayerMask.GetMask("Items")));

        foreach (Collider2D hit in hitEnemies)
        {
            if(hit.gameObject != player)
            {
                if (hit.gameObject.GetComponent<IDamageable>() != null)
                {
                    Debug.Log("Hit");
                    ac.PlaySound(audio_impact);
                    GameObject.Find("VirCam").GetComponent<VirCamStuff>().Shake(0.9f, 1.5f, 0.2f, 0f);
                    if (hit.gameObject.GetComponent<IDamageable>().Damage(damage))
                    {
                        KillConfirm(player);
                    }
                }

            }
        }
    }

    public bool Use_Grenade(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        if (Cooked)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                GameObject gre = Instantiate(GrenadeOBJ, attackPoint);
                gre.GetComponent<GrenadeObj>().PrepareGrenade(damage, CookTimer_Current);
                gre.transform.parent = null;
                return true;
            }
            CookTimer_Current -= Time.deltaTime;
            if(CookTimer_Current <= 0)
            {
                ac.PlaySound(audio_click, 0.2f);
                GameObject gre = Instantiate(GrenadeOBJ, attackPoint);
                gre.GetComponent<GrenadeObj>().PrepareGrenade(damage, CookTimer_Current);
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
    public void Tracer(RaycastHit2D hit, UnityEngine.Transform attackPoint)
    {
        TrailRenderer TrailBase = GameObject.Find("Trail").GetComponent<TrailRenderer>();
        TrailRenderer trail = Instantiate(TrailBase, attackPoint.position, Quaternion.identity);

        em.StartCoroutine(em.BulletTrailRoutine(trail, hit));
    }

    public void KillConfirm(GameObject player)
    {
        player.SendMessage("KillConfirmed");
    }

    public void CreateParticle(GameObject particles, RaycastHit2D point)
    {
        Debug.Log("Now Shooting blud");
        Instantiate(particles, point.point, Quaternion.LookRotation(point.normal, Vector3.left));
    }

    public string GetName()
    {
        return Name;
    }

    public string GetAmmo()
    {
        return CurrentAmmo.ToString();
    }

    public Sprite GetWeaponSprite()
    {
        return floorImage;
    }

    public Sprite GetWeaponSprite_Held()
    {
        return HeldImage;
    }
}

