using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class OnlineGameCoordinator : MonoBehaviourPunCallbacks
{
    public static OnlineGameCoordinator instance;
    public PhotonView PV;

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

    public void Awake()
    {
        playerManager.Clear();
        arenaList = Resources.Load<ArenaList>("ArenaList");
        instance = this;
    }

    public void RegisterPlayerManager(PlayerManager pm)
    {
        playerManager.Add(pm);
        print("Added " + pm.name);
        SetUpPlayerScore();
        if (PhotonNetwork.IsMasterClient && playerManager.Count == PhotonNetwork.PlayerList.Length)
        {
            LoadArena();
            PrepareForStart();
        }
    }

    private void SetUpPlayerScore()
    {
        for (int i = 0; i < playerManager.Count; i++)
        {
            points[i] = 0;
        }
        foreach (PlayerManager pm in playerManager)
        {
            pm.score = points;
        }
    }

    private void GetSpawns()
    {
        Spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Shuffle(Spawnpoints);
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    void SpawnPlayers()
    {
        int i = 0;
        foreach (PlayerManager pm in playerManager)
        {
            pm.SpawnPlayer(Spawnpoints[i].transform.position);
            playersAlive[i] = true;
            i++;
            pm.NextRoundNowLoaded();
        }
    }

    public void PlayerDied(int ply)
    {
        print("Player died: " + (ply - 1));
        PV.RPC("PlayerDiedRPC", RpcTarget.All, ply - 1);
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
                    PV.RPC("GameComplete", RpcTarget.All, points);
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
        for (int i = 0; i < playerManager.Count; i++)
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

    #region Start

    public void PrepareForStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartTheGame());
        }
    }

    public IEnumerator StartTheGame()
    {
        //Start Phase 1
        PV.RPC("Phase1RPC", RpcTarget.All);
        yield return new WaitForSeconds(5f);
        PV.RPC("Phase2RPC", RpcTarget.All);

        yield return new WaitForSeconds(1.5f);
        //Start countdown
        PV.RPC("CountdownRPC", RpcTarget.All, new object[] { "3", 0 });
        yield return new WaitForSeconds(1);
        PV.RPC("CountdownRPC", RpcTarget.All, new object[] { "2", 0 });
        yield return new WaitForSeconds(1);
        PV.RPC("CountdownRPC", RpcTarget.All, new object[] { "1", 0 });
        yield return new WaitForSeconds(1);
        PV.RPC("Phase3RPC", RpcTarget.All);
    }

    [PunRPC]
    public void Phase1RPC()
    {
        foreach (PlayerManager pm in playerManager)
        {
            pm.StartGame_phase1();
        }
    }

    [PunRPC]
    public void Phase2RPC()
    {
        foreach (PlayerManager pm in playerManager)
        {
            pm.StartGame_phase2();
        }
    }

    [PunRPC]
    public void CountdownRPC(string String, int i)
    {
        foreach (PlayerManager pm in playerManager)
        {
            pm.StringCountdown(String, i);
        }
    }

    [PunRPC]
    public void Phase3RPC()
    {
        foreach (PlayerManager pm in playerManager)
        {
            pm.StartGame_phase3();
        }
    }

    #endregion

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
    public void GameComplete(int[] points)
    {
        print("Everyone is dead [Finalized]");
        foreach (PlayerManager pm in playerManager)
        {
            pm.GameComplete(points);
        }

    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameRoutine());
    }
    [PunRPC]
    public void RestartRPC()
    {

        foreach (PlayerManager pm in playerManager)
        {
            pm.StartGame_phase0();
        }
    }
    public IEnumerator RestartGameRoutine()
    {
        //Fade to black
        SetUpPlayerScore();
        if (PhotonNetwork.IsMasterClient && playerManager.Count == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RestartRPC", RpcTarget.All);
            yield return new WaitForSeconds(2);
            LoadArena();
            PrepareForStart();
        }
    }


}
