using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour, IDamageable
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
    [SerializeField] private NetworkVariable<float> Health = new NetworkVariable<float>(100);
    [SerializeField] private NetworkVariable<Weapon> weapon;
    [SerializeField] private Weapon unhandedWeapon;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask itemMask;
    [SerializeField] private float pickupRadius;
    [SerializeField] private GameObject pickupPrefab;


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
        if (IsClient)
        {
            return;
        }


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

    }

    private void HandleCombat()
    {
        int result = 0;
        if (weapon != null) result = weapon.Value.UseWeapon(attackPoint, pAud);
        else unhandedWeapon.UseWeapon(attackPoint, pAud);

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
                wpn.GetComponent<WeaponPickup>().SetupPickup(weapon.Value);
                wpn.transform.parent = null;
            }

            if (newWep != null)
            {
                weapon.Value = newWep.GetComponent<WeaponPickup>().PickupWeapon();
            }
            else
            {
                weapon.Value = null;

            }
        }
    }

    public void Damage(float damage)
    {
        Health.Value -= damage;

        if(Health.Value <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = (Vector3)(MoveRealized * MoveSpeed);
    }
}
