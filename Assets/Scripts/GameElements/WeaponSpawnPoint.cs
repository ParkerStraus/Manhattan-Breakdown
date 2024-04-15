using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawnPoint : MonoBehaviour
{

    public int WeaponIndex;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject gun = PhotonNetwork.Instantiate("PhotonPrefabs/Random Weapon Pickup", transform.position, Quaternion.identity);
            gun.GetComponent<WeaponPickup>().WeaponIndex = WeaponIndex;
            gun.GetComponent<WeaponPickup>().FirstTimeStart = true;
        }
    }
}
