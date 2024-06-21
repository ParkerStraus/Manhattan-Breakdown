using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameElement : MonoBehaviourPunCallbacks
{
    public void DestroyObject()
    {
        photonView.RPC("DestroyObjectRPC", RpcTarget.All);
    }

    [PunRPC]
    public void DestroyObjectRPC()
    {
        Destroy(gameObject);
    }
}
