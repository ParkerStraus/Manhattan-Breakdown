using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnlineGameCoordinator : MonoBehaviourPunCallbacks
{
    public static OnlineGameCoordinator instance;
    [SerializeField] private GameObject[] Spawnpoints;
    [SerializeField] private List<PlayerManager> playerManager = new List<PlayerManager>();
    private void OnEnable()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            print("Now working with MasterClient");
            GetSpawns();
        }else print("Now working with other");
    }

    public void RegisterPlayerManager(PlayerManager pm)
    {
        playerManager.Add(pm);
        print("Added "+pm.name);
    }

    [PunRPC]
    private void SpawnPlayer()
    {
    }

    private void GetSpawns()
    {
        Spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }


}
