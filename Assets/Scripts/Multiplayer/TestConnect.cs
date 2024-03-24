using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
    [SerializeField] private MasterManager _MasterManager;
    // Start is called before the first frame update
    void Start()
    {
        print("Connected to server");
        PhotonNetwork.NickName = MasterManager.GameSettings.Nickname;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        print("Connected to server");
        print (PhotonNetwork.LocalPlayer.NickName);
        print(PhotonNetwork.GameVersion);

        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnedcted from server for reason" + cause.ToString());
        
    }
}
