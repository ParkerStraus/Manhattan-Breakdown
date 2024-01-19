using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = System.Random;
using UnityEngine.UIElements;

public class GameHandler : MonoBehaviour
{

    [Header("Pause Stuff")]
    bool CurrentlyPaused = false;
    public UnityEvent OnPause;
    public UnityEvent OnUnpause;

    [Header("Player Stuff")]
    public List<GameObject> players;
    public int playerAmt = 1;
    public bool[] playerAlive = { false, false, false, false};

    [Header("Debug Stuff")]
    public int DummyPlayers = 1;
    public GameObject dummyPrefab;


    public enum GameState
    {
        WaitingForPlayers,
        Countdown,
        InGame,
        GameEnded
    }

    [Header("Arena Stuff")]
    public string CurrentArena = null;
    public List<string> LastArenas;
    public ArenaList arenaList;
    public int[] points = { 0, 0, 0, 0 };


    [Header("Prefabs and Cameras")]
    public CinemachineVirtualCamera virtualCamera;
    public GameObject playerPrefab;
    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        arenaList = Resources.Load<ArenaList>("ArenaList");
        //StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnGamePause();
        }
    }

    void OnGamePause()
    {
        CurrentlyPaused = !CurrentlyPaused;
        if(CurrentlyPaused)
        {
            OnPause.Invoke();
        }
        else
        {
            OnUnpause.Invoke();
        }
    }
    public bool IsPaused()
    {
        return CurrentlyPaused;
    }

    public void StartGame()
    {
        GetMapReadyServerRPC();
    }

    public void OnKill(int id)
    {
        Debug.Log("Kill Confirmed on "+id);
        playerAlive[id] = false;

        //Check if only one is alive
        int pAmt = 0;
        for(int i = 0; i < 4; i++)
        {
            if (playerAlive[i] == true)
            {
                pAmt++;
            }
        }
        if(pAmt == 1)
        {
            //Go to round end
            StartCoroutine(NextRound());
        }
    }



    public void OnRoundEnd()
    {
        GetMapReadyServerRPC();
    }

    private IEnumerator NextRound()
    {
        Debug.Log("Next Round Starting");
        yield return new WaitForSeconds(1);


        for (int i = 0; i < 4; i++)
        {
            if (playerAlive[i] = true)
            {
                points[i]++;
            }
        }

        yield return new WaitForSeconds(2);


        OnRoundEnd();

    }

    [ServerRpc]
    void GetMapReadyServerRPC()
    {

        string ArenaName = arenaList.GetScene(-1);
        GetMapReadyClientRPC(ArenaName);

    }

    [ClientRpc]
    void GetMapReadyClientRPC(string map)
    {

        StartCoroutine(GoToNextArena(map));

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
        PreparePlayersForArena();
        CurrentArena = map;

    }

    public void PreparePlayersForArena()
    {
        string LoadOrder = "0123";
        Random r = new Random();
        char[] shuffledLoadOrder = LoadOrder.ToCharArray().OrderBy(s => (r.Next(2) % 2) == 0).ToArray();

        //Spawn Player
        GameObject[] spawn = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Debug.Log(shuffledLoadOrder[0]);
        
        //TODO spawn all players along spawn objects

        //DEBUG Populate with dummy players
        var player = Instantiate(playerPrefab, spawn[int.Parse(shuffledLoadOrder[0].ToString())].transform);
        player.transform.parent = null;
        playerAlive[0] = true;
        virtualCamera.Follow = player.transform;

        for(int i = 1; i <= DummyPlayers; i++)
        {
            playerAlive[i] = true;
            var dummyPlayer = Instantiate(dummyPrefab, spawn[int.Parse(shuffledLoadOrder[i].ToString())].transform);
            dummyPlayer.GetComponent<DummyPlayer>().PID = i;   
            dummyPlayer.transform.parent = null;
        }
    }
}
