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
    public int DummyPlayers;
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
    public int[] points = { -1, -1, -1, -1};


    [Header("Prefabs and Cameras")]
    public CinemachineVirtualCamera virtualCamera;
    public GameObject playerPrefab;
    public GameObject playerObject;

    [Header("UI")]
    public UnityEvent DisplayScoreBoard;
    public UnityEvent DisplayVHS;
    public UnityEvent HideVHS;
    public GameObject FinalScoreBoard;
    public MainUI MainUI;

    // Start is called before the first frame update
    void Start()
    {
        arenaList = Resources.Load<ArenaList>("ArenaList");
        //StartGame();
        playerAmt = playerAmt + DummyPlayers;

        for(int i = 0; i < playerAmt; i++)
        {
            points[i] = 0;
        }

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
        int pALive = 0;
        for(int i = 0; i < playerAmt; i++)
        {
            if (playerAlive[i] == true)
            {
                pALive++;
            }
        }
        if(pALive == 1)
        {
            int p = -1;
            for (int i = 0; i < 4; i++)
            {
                if (playerAlive[i] == true)
                {
                    p = i;
                    break;
                }
            }
            bool rflag = true;
            points[p]++;
            for(int x =0; x<points.Length; x++)
            {
                if (points[x] >= 5)
                {
                    StartCoroutine(GameComplete());
                    rflag = false;
                    break;
                }

            }



            if (rflag == true) StartCoroutine(NextRound());
            //Go to round end

        }
    }



    public void OnRoundEnd()
    {
        GetMapReadyServerRPC();
    }

    private IEnumerator NextRound()
    {
        Debug.Log("Next Round Starting");
        GetComponent<MusicHandler>().TriggerFilter(500f, 0.5f);

        yield return new WaitForSeconds(1);
        DisplayVHS.Invoke();
        DisplayScoreBoard.Invoke();
        yield return new WaitForSeconds(2.25f);

        GetComponent<MusicHandler>().TriggerFilter(22000f, 0.5f);
        yield return new WaitForSeconds(0.50f);
        HideVHS.Invoke();


        OnRoundEnd();

    }



    private IEnumerator GameComplete()
    {
        Debug.Log("Next Round Starting");
        DisplayVHS.Invoke();
        GetComponent<MusicHandler>().TriggerFilter(300f, 0.5f);

        yield return new WaitForSeconds(3);
        FinalScoreBoard.SetActive(true);
        FinalScoreBoard.GetComponent<FinalResults>().SetupScoreBoard(scores: points);
        

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

    public int[] GetScore()
    {
        return points;
    }

    public void UpdateMainUI(string[] Weapon)
    {
        MainUI.UpdateMainUI(Weapon);
    }
}
