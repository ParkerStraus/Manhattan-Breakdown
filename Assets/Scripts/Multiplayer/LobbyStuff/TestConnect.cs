using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
    [SerializeField] private MasterManager _MasterManager;
    public static bool Connected = false;
    public GameObject LoadingScreen;
    // Start is called before the first frame update
    void Start()
    {
        print("Waiting on connection");
        PhotonNetwork.NickName = MasterManager.GameSettings.Nickname;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
        if (Connected)
        {
            LoadingScreen.SetActive(false);
        }
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        print("Connected to server");
        print (PhotonNetwork.LocalPlayer.NickName);
        print(PhotonNetwork.GameVersion);
        Connected = true;
        PhotonNetwork.JoinLobby();
        LoadingScreen.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Connected = false;
        print("Disconnedcted from server for reason" + cause.ToString());
        
    }
}
