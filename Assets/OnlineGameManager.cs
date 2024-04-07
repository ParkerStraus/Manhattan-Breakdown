using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

public class OnlineGameManager : MonoBehaviourPunCallbacks, IGameHandler
{
    [SerializeField] private GameObject[] spawnpoints;
    PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        GetSpawnpoints();
        PV = GetComponent<PhotonView>();
        if(PhotonNetwork.IsMasterClient == false){
            Debug.Log("This device is not the admin");
            return;
        }
        Debug.Log("This device is the admin");
        SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateController()
    {

    }

    void GetSpawnpoints()
    {
        spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    private void SpawnPlayers()
    {
        int[] spawns = { 0, 1, 2, 3 };

        ShuffleArray(spawns);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("Sending spawn " + spawnpoints[i]);
            PV.RPC("SpawnThePlayer", PhotonNetwork.PlayerList[i], spawns[i]);
        }
    }

    void ShuffleArray<T>(T[] array)
    {
        // Using Fisher-Yates shuffle algorithm
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T temp = array[k];
            array[k] = array[n];
            array[n] = temp;
        }
    }

    [PunRPC]
    private void SpawnThePlayer(int spawnpoint)
    {
        if (PV.IsMine)
        {
            Debug.LogWarning("Spawning player at " + spawnpoint);
            var player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoints[spawnpoint].transform.position, Quaternion.identity);
            player.GetComponent<Player>().SetIGH(this);
        }
    }

    public bool IsMine()
    {
        return PV.IsMine;
    }
}
