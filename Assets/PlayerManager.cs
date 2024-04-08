using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

public class PlayerManager : MonoBehaviourPunCallbacks, IGameHandler
{
    PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            var pla = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector3.zero, Quaternion.identity);
            pla.GetComponent<Player>().SetIGH(this);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Is Master Client");
            }
        }
    }

    public bool IsMine()
    {
        return PV.IsMine;
    }
}
