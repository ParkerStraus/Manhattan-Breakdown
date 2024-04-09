using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;

public class WeaponPickup : MonoBehaviourPunCallbacks
{
    [SerializeField] private int WeaponIndex;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Weapon weapon;
    [SerializeField] private int Ammo;
    [SerializeField] private int AmmoInClip;

    [SerializeField] private float Rotate;
    [SerializeField] private float RotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        if(weapon == null)
        {
            weapon = Instantiate(Resources.Load<WeaponList>("WeaponData/WeaponList")
                                     .GetWeapon(WeaponIndex));
            
        }
        weapon.Initialize();

        sprite.sprite = weapon.GetWeaponSprite();

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
}
