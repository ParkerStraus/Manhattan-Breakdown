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
    public int PlayerAmt { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        RoomManager.instance.RegisterPlayerManager(this);
        PV = GetComponent<PhotonView>();
    }

    public bool IsMine()
    {
        return PV.IsMine;
    }

    public void SpawnPlayer(Vector3 pos)
    {
        PV = GetComponent<PhotonView>();
        PV.RPC("SpawnPlayerLocal", RpcTarget.All, pos);
    }


    public void OnKill(int whoDied)
    {
        if(PV.IsMine)
        {
            RoomManager.instance.PlayerDied(PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    [PunRPC]
    public void SpawnPlayerLocal(Vector3 pos)
    {

        if (PV.IsMine)
        {
            var pla = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), pos, Quaternion.identity);
            
            pla.GetComponent<Player>().SetIGH(this);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Is Master Client");
            }
        }
    }
}
