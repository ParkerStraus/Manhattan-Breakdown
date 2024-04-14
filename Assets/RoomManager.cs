using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{

    #region Connection Stuff

    public static RoomManager instance;
    public PhotonView PV;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        PV = GetComponent<PhotonView>();
        instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            
        }
    }

    #endregion

    #region GameCentric Info

    //Game
    [SerializeField] private GameObject[] Spawnpoints;
    [SerializeField] private List<PlayerManager> playerManager = new List<PlayerManager>();
    [SerializeField] private bool[] playersAlive = new bool[] { false, false, false, false };
    [SerializeField] private int[] points = { -1, -1, -1, -1 };

    public void RegisterPlayerManager(PlayerManager pm)
    {
        playerManager.Add(pm);
        print("Added " + pm.name);

        if (PhotonNetwork.IsMasterClient && playerManager.Count == PhotonNetwork.PlayerList.Length)
        {
            GetSpawns();
            SpawnPlayers();
        }
    }

    private void GetSpawns()
    {
        Spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    void SpawnPlayers()
    {
        int i = 0;
        foreach(PlayerManager pm in playerManager)
        {
            pm.SpawnPlayer(Spawnpoints[i].transform.position);
            playersAlive[i] = true;
            i++;

        }
    }

    public void PlayerDied(int ply)
    {
        print("Player died: "+(ply-1));
        PV.RPC("PlayerDiedRPC", RpcTarget.MasterClient, ply-1);
    }

    [PunRPC]
    public void PlayerDiedRPC(int ply)
    {
        print("Player died: " + ply);
        playersAlive[ply] = false;
        int playersLeft = 0;
        for (int i = 0; i < playersAlive.Length; i++)
        {
            if (playersAlive[i] == true)
            {
                playersLeft++;
            }
        }

        if (playersLeft <= 1 && PhotonNetwork.IsMasterClient)
        {
            int p = -1;
            for (int i = 0; i < 4; i++)
            {
                if (playersAlive[i] == true)
                {
                    p = i;
                    break;
                }
            }
            bool rflag = true;
            points[p]++;
            for (int x = 0; x < points.Length; x++)
            {
                if (points[x] >= 5)
                {
                    PV.RPC("GameComplete", RpcTarget.All);
                    rflag = false;
                    break;
                }

            }

            if (rflag == true) PV.RPC("NextRound", RpcTarget.All);
        }
    }


    [PunRPC]
    public void NextRound()
    {
        print("Everyone is dead [Finalized]");

        foreach (PlayerManager pm in playerManager)
        {
            pm.NextRound();
        }
    }

    [PunRPC]
    public void GameComplete()
    {
        print("Everyone is dead [Finalized]");
    }
    

    #endregion

}
