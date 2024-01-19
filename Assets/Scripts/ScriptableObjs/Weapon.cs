using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    public float ConeSegment;
    public int BurstAmount;
    public int BurstCurrent = 0;
    public float BurstInterval;
    public int ConeRayAmount;


    [Header("Aesthetic stuff here")]
    [SerializeField] private Sprite floorImage;
    [SerializeField] private AudioClip audio_gunshot;
    [SerializeField] private AudioClip audio_click;
    [SerializeField] private AudioClip audio_impact;
    [SerializeField] private GameObject Impact;

    public void Initialize()
    {
        CurrentAmmo = MaxAmmo;
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
                                    GunShot(attackPoint, ac);
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
                                    GunShot(attackPoint, ac);
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
                                        GunShot(attackPoint, ac);
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
                                    GunShot(attackPoint, ac);
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
                return -1;
                break;
        }
        AttackTimer += Time.deltaTime;
        return 0;
    }

    public void GunShot(UnityEngine.Transform attackPoint, PlayerAudio ac)
    {
        if(CurrentAmmo > 0)
        {
            CurrentAmmo--;
            ac.PlaySound(audio_gunshot);
            //Debug.Log(attackPoint.rotation.eulerAngles.z);
            Debug.DrawRay(attackPoint.position,
                Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z) * Vector2.right, Color.white, 0.1f);
            //Shoot Ray
            RaycastHit2D[] hits = Physics2D.RaycastAll(attackPoint.position,
                Quaternion.Euler(0, 0, attackPoint.rotation.eulerAngles.z) * Vector2.right, 9999, ~(LayerMask.GetMask("Items")));
            foreach (RaycastHit2D hit in hits)
            {
                
                Instantiate(Impact, hit.point, Quaternion.LookRotation(hit.normal, Vector3.left));
                if (hit.collider.gameObject.GetComponent<IDamageable>() != null)
                {
                    if (hit.collider.gameObject.GetComponent<IDamageable>().Damage(damage))
                    {
                        KillConfirm();
                    }
                }
                break;
            }
        }
    }

    public void Melee(UnityEngine.Transform attackPoint, PlayerAudio ac, GameObject player)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, ~(LayerMask.GetMask("Items")));

        foreach (Collider2D hit in hitEnemies)
        {
            if(hit.gameObject != player)
            {
                Debug.Log("Hit");
                ac.PlaySound(audio_impact);
                if (hit.gameObject.GetComponent<IDamageable>() != null)
                {
                    if (hit.gameObject.GetComponent<IDamageable>().Damage(damage))
                    {
                        KillConfirm();
                    }
                }

            }
        }
    }

    public void KillConfirm()
    {

    }

    public Sprite GetWeaponSprite()
    {
        return floorImage;
    }
}

