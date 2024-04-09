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
            var go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            
        }
    }

    #endregion

    #region GameCentric Info

    //Game
    [SerializeField] private GameObject[] Spawnpoints;
    [SerializeField] private List<PlayerManager> playerManager = new List<PlayerManager>();
    [SerializeField] private bool[] playersAlive = new bool[] { false, false, false, false };

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
        print("Player died: "+ply);
        playersAlive[ply] = false;
        int playersLeft = 0;
        for (int i = 0; i < playersAlive.Length; i++)
        {
            if (playersAlive[i] = true)
            {
                playersLeft++;
            }
        }

        if (playersLeft <= 1)
        {
            PV.RPC("RoundWin", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RoundWin()
    {
        print("Everyone is dead");
    }
    

    #endregion

}
