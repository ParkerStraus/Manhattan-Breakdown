using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EntitySpawnPoint : MonoBehaviourPunCallbacks
{
    public string ObjectName;
    // Start is called before the first frame update
    void Start()
    {
        //Work from here
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("PhotonPrefabs/"+ObjectName, transform.position, Quaternion.identity);

        }
    }
}
