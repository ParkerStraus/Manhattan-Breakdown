using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using UnityEditor;
using Photon.Pun;
using System;

public struct PlayerData
{
    public bool offline;
    public bool Moving;
    public PoseType poseType;
    public bool attacking;

}

public class Player : IForceObject, IDamageable
{

    [Header("Game Components")]
    public static Player localInstance;
    [SerializeField] public IGameHandler gh;
    [SerializeField] private PlayerAudio pAud;
    [SerializeField] private PlayerAnimation anim;
    [SerializeField] private PhotonView PV;
    [SerializeField] private bool offline = false;

    [Header("Movement")]
    [SerializeField] private NetworkVariable<Vector2> Position;
    [SerializeField] private NetworkVariable<Quaternion> Rotation;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private Vector2 MoveRealized;
    [SerializeField] private float MoveInterp;
    [SerializeField] private float RotationRealized;
    [SerializeField] private float RotationSpeed;

    [Header("Health and Guns")]
    [SerializeField] private bool Dead = false;
    [SerializeField] private float Health = 100;
    [SerializeField] private Weapon weapon = null;
    [SerializeField] private Sprite[] weaponSprites;
    public int WeaponDraw_Index;
    public bool WeaponDraw_Flipped;
    [SerializeField] private SpriteRenderer WeaponDraw;
    [SerializeField] private Weapon unhandedWeapon;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask itemMask;
    [SerializeField] private float pickupRadius;
    [SerializeField] private GameObject pickupPrefab;

    [SerializeField] private PlayerData _playerData;

    [Header("Damageable Stuff")]
    [SerializeField] private GameObject impactEffect;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(gh == null)
        {
            gh = GameObject.Find("Main Camera").GetComponent<IGameHandler>();
        }
        pAud = GetComponent<PlayerAudio>();
        anim = GetComponent<PlayerAnimation>();
        PV = GetComponent<PhotonView>();
        SendWeaponInfo();
        if(PV.IsMine)
        {
            localInstance = this;
            FieldOfView.Instance.SetEnabledFOV(true);
        }
        else
        {
            FieldOfView.Instance.SetEnabledFOV(false);
        }
    }

    public void SetOffline()
    {
        offline = true;
        _playerData.offline = true;
    }

    public void SetIGH(IGameHandler input)
    {
        Debug.Log("Now setting to " + input);
        gh = input;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine && !offline) 
        {
            return;
        }
        if ((!PauseMenu.instance.IsPaused())  && !Dead)
        {
            HandleMovement();
            HandleCombat();
        }
        else
        {
            //Set Character to zero
            MoveRealized = Vector2.Lerp(MoveRealized, Vector2.zero, MoveInterp * Time.fixedDeltaTime);
        }
        base.Update();
        FieldOfView.Instance.SetAimDirection(transform.rotation.eulerAngles);
        FieldOfView.Instance.SetOrigin(this.transform.position);

    }

    private void HandleMovement()
    {
        Vector2 MoveVector = Vector2.zero;
        
        if (gh.CanthePlayersMove())
        {
            //Get Move Vector
            MoveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            //Set Realized Speed
            MoveRealized = Vector2.Lerp(MoveRealized, MoveVector, MoveInterp * Time.fixedDeltaTime);

        }
        //Rotate Based on where the camera is looking
        //Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Vector3 WorldPoint = this.gameObject.transform.position;
        //Aiming shit
        Vector3 AimPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float RotationRealized = Mathf.Atan2(WorldPoint.y - AimPoint.y, WorldPoint.x - AimPoint.x) * 180 / Mathf.PI + 180;
        float DirectionAngle = Vector2.Angle((Vector2)transform.position, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //Debug.Log((Vector2)transform.position +"   " + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) );
        transform.rotation = Quaternion.Euler(0, 0, RotationRealized);
        if (MoveVector.sqrMagnitude > 0)
        {
            _playerData.Moving = true;
        }
        else { _playerData.Moving = false; }

    }

    private void HandleCombat()
    {
        _playerData.attacking = false;
        if (gh.CanthePlayersMove())
        {
            int result = 0;
            if (weapon != null)
            {
                result = ((IWeapon)weapon).UseWeapon(attackPoint, this.gameObject);
                _playerData.poseType = weapon._poseType;
                if (weapon.Attacking)
                {
                    _playerData.attacking = true;
                }
                else
                {
                    _playerData.attacking = false;

                }
                //Debug.Log(_playerData.poseType);
                weaponSprites = weapon.GetWeaponSprite_Held();
                WeaponDraw.sprite = weaponSprites[WeaponDraw_Index];
                PV.RPC("SetWeaponSpriteRPC", RpcTarget.Others, weapon.Index);
            }
            else
            {
                ((IWeapon)unhandedWeapon).UseWeapon(attackPoint, this.gameObject);
                if (Input.GetButtonDown("Fire1"))
                {
                    _playerData.attacking = true;
                }
                _playerData.poseType = PoseType.None;
                WeaponDraw.sprite = null;
                PV.RPC("SetWeaponSpriteRPC", RpcTarget.Others, -1);
            }

            //Remove Consumeables
            if (result == -1)
            {
                weapon = null;
                SendWeaponInfo();
            }

            //Interact with dropping weapons
            if (Input.GetButtonDown("Fire2"))
            {
                Collider2D[] hitItems = Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemMask);

                Debug.Log(hitItems.ToString());
                GameObject newWep = null;
                foreach (Collider2D hitItem in hitItems)
                {
                    if (hitItem.GetComponent<WeaponPickup>() != null)
                    {
                        newWep = hitItem.GameObject();

                        break;
                    }
                }

                if (weapon != null)
                {
                    try { 
                        GameObject wpn = PhotonNetwork.Instantiate("PhotonPrefabs/"+pickupPrefab.name, transform.position, Quaternion.identity);
                        wpn.GetComponent<WeaponPickup>().SetupPickup(weapon);
                        wpn.transform.parent = null;
                    }
                    catch
                    {
                        GameObject wpn = Instantiate(pickupPrefab, transform);
                        wpn.GetComponent<WeaponPickup>().SetupPickup(weapon);
                        wpn.transform.parent = null;
                    }
                }

                if (newWep != null)
                {
                    weapon = newWep.GetComponent<WeaponPickup>().PickupWeapon();
                }
                else
                {
                    weapon = null;

                }
            }
            SendWeaponInfo();
        }
        //Change Pose based on gun
    }

    [PunRPC] public void SetWeaponSpriteRPC(int weapon)
    {
        if(weapon == -1)
        {
            WeaponDraw.sprite = null;
        }
        else
        {
            weaponSprites = Resources.Load<WeaponList>("WeaponData/WeaponList")
                                             .GetWeapon(weapon).GetWeaponSprite_Held();
            WeaponDraw.sprite = weaponSprites[WeaponDraw_Index];
        }
    }

    public PlayerData GetPlayerData()
    {
        return _playerData;
    }
    public bool Damage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            if (Dead == true)
            {
            }
            else
            {

                Dead = true;
                WeaponDraw.sprite = null;
                anim.OnDeath();
                PV.RPC("Die", RpcTarget.All);
                return true;
            }
        }
        PV.RPC("SetHealth", RpcTarget.All, Health);

        return false;
    }

    [PunRPC]
    public void SetHealth(float Health)
    {
        this.Health = Health;
    }

    [PunRPC]
    public void Die()
    {
        Dead = true;
        WeaponDraw.sprite = null;

        if (PV.IsMine)
        {
            Debug.Log("dead now");
            gh.OnKill(0);
        }

    }

    private void FixedUpdate()
    {
        rb.velocity = (Vector3)(MoveRealized * MoveSpeed + MoveForce);
    }

    public void SendWeaponInfo()
    {
        //Debug.Log(weapon.Name);
        string[] value = new string[2];
        int UIOverride;
        try { 
            value[0] = weapon.GetName();
            value[1] = weapon.GetAmmoString();
            UIOverride = 1;
        }
        catch(NullReferenceException e)
        {
            value[0] = "";
            value[1] = "";
            UIOverride = 0;

        }
        //Debug.Log(UIOverride);
        MainUI.Instance.UpdateMainUI(UIOverride, value);
    }

    public GameObject GetImpactEffect()
    {
        return impactEffect;
    }

    public void CreateImpact(RaycastHit2D hit)
    {
        PV.RPC("CreateImpactRPC", RpcTarget.All, new object[] { hit.point, Quaternion.LookRotation(hit.normal, Vector3.left) });
    }

    [PunRPC]

    public void CreateImpactRPC(Vector2 pos, Quaternion rot)
    {
        Instantiate(GetImpactEffect(), pos, rot);
    }
}

