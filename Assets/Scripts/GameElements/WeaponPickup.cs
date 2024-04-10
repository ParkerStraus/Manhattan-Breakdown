using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;

public class WeaponPickup : MonoBehaviourPunCallbacks
{
    private PhotonView PV;

    [SerializeField] private int WeaponIndex;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Weapon weapon;
    [SerializeField] private int Ammo;

    [SerializeField] private float Rotate;
    [SerializeField] private float RotateSpeed;
    // Start is called before the first frame update
    void Start()
    {

        if(WeaponIndex == -1)
        {
            WeaponIndex = Resources.Load<WeaponList>("WeaponData/WeaponList").GetRandomWeaponIndex();
            weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                     .GetWeapon(WeaponIndex));

            
        }
        weapon.Initialize();

        sprite.sprite = weapon.GetWeaponSprite();

    }

    [PunRPC]
    public void SetupRPC(int weaponIndex)
    {
        weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                     .GetWeapon(WeaponIndex));
        weapon.Initialize();
    }

    private void Update()
    {
        Rotate += RotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, Rotate);
    }

    public Weapon PickupWeapon()
    {
        Destroy(this.gameObject);
        return weapon;
    }
    public void SetupPickup(Weapon weapon)
    {
        this.weapon = weapon;

    }

    public void SetupPickup(int weapon, int Ammo)
    {
        this.weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                     .GetWeapon(weapon));
        if(this.weapon.GetType() == typeof(Gun))
        {
            ((Gun)this.weapon).CurrentAmmo = Ammo;
        }
        PV.RPC("SetupPickupRPC", RpcTarget.Others, new object[] {weapon, Ammo});
    }

    public void SetupPickupRPC(int weapon, int Ammo)
    {
        this.weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                     .GetWeapon(weapon));
        if (this.weapon.GetType() == typeof(Gun))
        {
            ((Gun)this.weapon).CurrentAmmo = Ammo;
        }
    }
}
