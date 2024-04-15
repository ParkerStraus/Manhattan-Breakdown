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
        arenaList = Resources.Load<ArenaList>("ArenaList");
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


    [Header("Arena Stuff")]
    public string CurrentArena = null;
    public List<string> LastArenas;
    public ArenaList arenaList;
    [SerializeField] private int[] points = { -1, -1, -1, -1 };

    //Game
    [SerializeField] private GameObject[] Spawnpoints;
    [SerializeField] private List<PlayerManager> playerManager = new List<PlayerManager>();
    [SerializeField] private bool[] playersAlive = new bool[] { false, false, false, false };
    [SerializeField] private bool[] SyncLock = { true, true, true, true };

    public void RegisterPlayerManager(PlayerManager pm)
    {
        playerManager.Add(pm);
        print("Added " + pm.name);
        SetUpPlayerScore();

        if (PhotonNetwork.IsMasterClient && playerManager.Count == PhotonNetwork.PlayerList.Length)
        {
            LoadArena();
        }
    }

    private void SetUpPlayerScore()
    {
        for(int i = 0; i < playerManager.Count; i++)
        {
            points[i] = 0;
        }
        foreach(PlayerManager pm in playerManager)
        {

            pm.score = points;
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
        PV.RPC("PlayerDiedRPC", RpcTarget.All, ply-1);
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

            if (rflag == true) PV.RPC("NextRoundRPC", RpcTarget.All, points);
        }
    }


    [PunRPC]
    public void NextRoundRPC(int[] points)
    {
        this.points = points;
        print("Everyone is dead Next round");
        for(int i = 0; i < playerManager.Count; i++)
        {
            SyncLock[i] = false;
        }
        foreach (PlayerManager pm in playerManager)
        {
            pm.NextRound(points);
        }
    }

    public void PlayerNowInTransition(int player)
    {
        PV.RPC("PlayerNowInTransitionRPC", RpcTarget.All, player);
    }

    [PunRPC]
    public void PlayerNowInTransitionRPC(int player)
    {
        SyncLock[player] = true;
        if (PhotonNetwork.IsMasterClient)
        {
            bool canTransition = true;
            for (int i = 0; i < SyncLock.Length; i++)
            {
                if (SyncLock[i] == false)
                {
                    canTransition = false;
                }
            }
            if (canTransition)
            {
                LoadArena();
            }
        }
    }

    public void LoadArena()
    {
        Debug.Log("Now loading new level");
        string ArenaName = arenaList.GetScene(-1);
        PV.RPC("LoadArenaRPC", RpcTarget.All, ArenaName);
    }

    [PunRPC]
    public void LoadArenaRPC(string ArenaName)
    {
        StartCoroutine(GoToNextArena(ArenaName));
    }
    public IEnumerator GoToNextArena(string map)
    {

        if (CurrentArena != "")
        {
            SceneManager.UnloadScene(CurrentArena);
        }
        AsyncOperation scene = SceneManager.LoadSceneAsync(map, LoadSceneMode.Additive);

        while (!scene.isDone)
        {
            Debug.Log("loading new arena");
            yield return null;
        }
        //Clear all Particles
        GameObject[] particles = GameObject.FindGameObjectsWithTag("Particles");
        foreach (GameObject particle in particles)
        {
            Destroy(particle);
        }
        CurrentArena = map;
        if (PhotonNetwork.IsMasterClient)
        {
            GetSpawns();
            SpawnPlayers();
        }

    }

    [PunRPC]
    public void GameComplete()
    {
        print("Everyone is dead [Finalized]");
    }
    

    #endregion

}
