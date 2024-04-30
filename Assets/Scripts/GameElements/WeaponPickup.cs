using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;

public class WeaponPickup : MonoBehaviourPunCallbacks
{
    private PhotonView PV;

    [SerializeField] public int WeaponIndex;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Weapon weapon;
    [SerializeField] private int Ammo;
    [SerializeField] public bool FirstTimeStart;

    [SerializeField] private float Rotate;
    [SerializeField] private float RotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (FirstTimeStart)
        {
            if (WeaponIndex == -1 && weapon == null)
            {
                WeaponIndex = Resources.Load<WeaponList>("WeaponData/WeaponList").GetRandomWeaponIndex();
                weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                         .GetWeapon(WeaponIndex));

                Debug.Log("Now initializing random weapon: " +weapon.name);

                weapon.Initialize();
                if (this.weapon.GetType() == typeof(Gun)) Ammo = ((Gun)weapon).GetAmmoCount();

            }
            else if (weapon == null)
            {
                weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                         .GetWeapon(WeaponIndex));

                weapon.Initialize();
                if (this.weapon.GetType() == typeof(Gun)) Ammo = ((Gun)weapon).GetAmmoCount();
            }

            sprite.sprite = weapon.GetWeaponSprite();
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("SetupRPC", RpcTarget.Others, WeaponIndex);
            }
        }
    }

    [PunRPC]
    public void SetupRPC(int weaponIndex)
    {

        weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                     .GetWeapon(weaponIndex));
        weapon.Initialize();

        Debug.Log("Now initializing weapon: " + weapon.name);
        if (this.weapon.GetType() == typeof(Gun)) Ammo = ((Gun)weapon).GetAmmoCount();
        sprite.sprite = weapon.GetWeaponSprite();
    }

    private void Update()
    {
        Rotate += RotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, Rotate);
    }

    public Weapon PickupWeapon()
    {
        PV.RPC("PickupWeaponRPC", RpcTarget.Others);
        Destroy(this.gameObject);
        return weapon;
    }

    [PunRPC]
    public void PickupWeaponRPC()
    {
        Destroy(this.gameObject);
    }

    public void SetupPickup(Weapon weapon)
    {
        PV = GetComponent<PhotonView>();
        this.weapon = weapon;
        if (this.weapon.GetType() == typeof(Gun)) Ammo = ((Gun)weapon).GetAmmoCount();

        sprite.sprite = weapon.GetWeaponSprite();

        PV.RPC("SetupPickupRPC", RpcTarget.Others, new object[] { weapon.Index, Ammo });

    }

    public void SetupPickup(int weapon, int Ammo)
    {
        PV = GetComponent<PhotonView>();
        this.weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                     .GetWeapon(weapon));

        this.Ammo = Ammo;

        if (this.weapon.GetType() == typeof(Gun))
        {
            ((Gun)this.weapon).CurrentAmmo = Ammo;
        }
        sprite.sprite = this.weapon.GetWeaponSprite();
        PV.RPC("SetupPickupRPC", RpcTarget.Others, new object[] {weapon, Ammo});
    }

    [PunRPC]
    public void SetupPickupRPC(int weapon, int Ammo)
    {
        PV = GetComponent<PhotonView>();
        this.weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                     .GetWeapon(weapon));
        this.Ammo = Ammo;

        if (this.weapon.GetType() == typeof(Gun))
        {
            ((Gun)this.weapon).CurrentAmmo = Ammo;
        }
        sprite.sprite = this.weapon.GetWeaponSprite();
    }
}
