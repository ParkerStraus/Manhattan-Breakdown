using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnlineGameCoordinator : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] Spawnpoints;
    [SerializeField] private PlayerManager[] playerManager;
    private void OnEnable()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            print("Now working with MasterClient");
            GetSpawns();
            GetPlayerManagers();
        }else print("Now working with other");
    }

    private void GetPlayerManagers()
    {
        playerManager = (PlayerManager[])FindObjectsOfType(typeof(PlayerManager));
    }

    private void GetSpawns()
    {
        Spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }
}
