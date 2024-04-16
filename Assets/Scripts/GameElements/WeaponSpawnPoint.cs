using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawnPoint : MonoBehaviour
{

    public int WeaponIndex;
    public GameObject weapon;
    public bool Delayed;
    public bool Initialized;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            print("Now Initializing Weapon " + WeaponIndex);
            GameObject gun = PhotonNetwork.Instantiate("PhotonPrefabs/Random Weapon Pickup", transform.position, Quaternion.identity);
            gun.GetComponent<WeaponPickup>().WeaponIndex = WeaponIndex;
            gun.GetComponent<WeaponPickup>().FirstTimeStart = true;
            gun.transform.parent = null;
            Destroy(this.gameObject);
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.OfflineMode && !Initialized)
        {
            try
            {
                print("Now Initializing Weapon " + WeaponIndex);
                GameObject gun = PhotonNetwork.Instantiate("PhotonPrefabs/Random Weapon Pickup", transform.position, Quaternion.identity);
                gun.GetComponent<WeaponPickup>().WeaponIndex = WeaponIndex;
                gun.GetComponent<WeaponPickup>().FirstTimeStart = true;
                gun.transform.parent = null;
                Initialized = true;
                Destroy(this.gameObject);
                this.GetComponent<SpriteRenderer>().enabled = false;
            }
            catch
            {
                GameObject gun = GameObject.Instantiate(weapon, transform);
                gun.GetComponent<WeaponPickup>().WeaponIndex = WeaponIndex;
                gun.GetComponent<WeaponPickup>().FirstTimeStart = true;
                gun.transform.parent = null;
                Initialized = true;
                Destroy(this.gameObject);
                this.GetComponent<SpriteRenderer>().enabled = false;
            }
            Initialized = true;
        }
    }
}
