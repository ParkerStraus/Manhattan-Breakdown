using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Unity.Multiplayer.Tools.NetStatsMonitor;

public struct PlayerData
{
    public bool Moving;
    public PoseType poseType;

}

public class Player : MonoBehaviour, IDamageable
{

    [Header("Game Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameHandler gh;
    [SerializeField] private PlayerAudio pAud;

    [Header("Movement")]
    [SerializeField] private NetworkVariable<Vector2> Position;
    [SerializeField] private NetworkVariable<Quaternion> Rotation;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private Vector2 MoveRealized;
    [SerializeField] private float MoveInterp;
    [SerializeField] private float RotationSpeed;

    [Header("Health and Guns")]
    [SerializeField] private float Health = 100;
    [SerializeField] private Weapon weapon = null;
    [SerializeField] private Weapon unhandedWeapon;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask itemMask;
    [SerializeField] private float pickupRadius;
    [SerializeField] private GameObject pickupPrefab;


    private PlayerData _playerData;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gh = GameObject.Find("Main Camera").GetComponent<GameHandler>();
        pAud = GetComponent<PlayerAudio>();
    }

    // Update is called once per frame
    void Update()
    {


        if (!gh.IsPaused())
        {
            HandleMovement();
            HandleCombat();
        }
        else
        {
            //Set Character to zero
            MoveRealized = Vector2.Lerp(MoveRealized, Vector2.zero, MoveInterp * Time.deltaTime);
        }


    }

    private void HandleMovement()
    {
        //Get Move Vector
        Vector2 MoveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Set Realized Speed
        MoveRealized = Vector2.Lerp(MoveRealized, MoveVector, MoveInterp * Time.deltaTime);

        //Rotate Based on where the camera is looking
        //Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Vector3 WorldPoint = this.gameObject.transform.position;
        //Aiming shit
        Vector3 AimPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float AimAngle = Mathf.Atan2(WorldPoint.y - AimPoint.y, WorldPoint.x - AimPoint.x) * 180 / Mathf.PI + 180;
        float DirectionAngle = Vector2.Angle((Vector2)transform.position, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //Debug.Log((Vector2)transform.position +"   " + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) );
        transform.rotation = Quaternion.Euler(0, 0, AimAngle);
        if(MoveVector.sqrMagnitude > 0)
        {
            _playerData.Moving = true;
        }
        else { _playerData.Moving = false;}

    }

    private void HandleCombat()
    {
        int result = 0;
        if (weapon != null) {
            result = weapon.UseWeapon(attackPoint, pAud, this.gameObject);
            _playerData.poseType = weapon._poseType;
        }
        else
        {
            unhandedWeapon.UseWeapon(attackPoint, pAud, this.gameObject);
            _playerData.poseType = PoseType.None;
        }

        //Remove Consumeables
        if (result == -1)
        {
            weapon = null;
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

            if(weapon != null)
            {
                GameObject wpn = GameObject.Instantiate(pickupPrefab, transform);
                wpn.GetComponent<WeaponPickup>().SetupPickup(weapon);
                wpn.transform.parent = null;
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

        //Change Pose based on gun
    }

    public PlayerData GetPlayerData()
    {
        return _playerData;
    }
    public bool Damage(float damage)
    {
        Health -= damage;

        if(Health <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        MovePlayerServerRpc();
    }

    [ServerRpc]
    private void MovePlayerServerRpc()
    {
        rb.velocity = (Vector3)(MoveRealized * MoveSpeed);
        ApplyMovePlayerClientRpc();

    }

    [ClientRpc]
    private void ApplyMovePlayerClientRpc()
    {

        rb.velocity = (Vector3)(MoveRealized * MoveSpeed);
    }
}
