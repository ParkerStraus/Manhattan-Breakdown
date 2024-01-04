using Cinemachine;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameHandler : NetworkBehaviour
{
    bool CurrentlyPaused = false;
    public UnityEvent OnPause;
    public UnityEvent OnUnpause;

    public List<GameObject> players;
    public int playerAmt = 1;

    public enum GameState
    {
        WaitingForPlayers,
        Countdown,
        InGame,
        GameEnded
    }

    public string CurrentArena = null;
    public List<string> LastArenas;
    public ArenaList arenaList;

    public CinemachineVirtualCamera virtualCamera;
    public GameObject playerPrefab;
    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        arenaList = Resources.Load<ArenaList>("ArenaList");
        StartGame();
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
        AsyncOperation scene = SceneManager.LoadSceneAsync(map, LoadSceneMode.Additive);

        while (!scene.isDone)
        {
            Debug.Log("loading new arena");
            yield return null;
        }
        PreparePlayersForArena();
        if(CurrentArena != null)
        {

        }
        CurrentArena = map;

    }

    public void PreparePlayersForArena()
    {
        
        GameObject[] spawn = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Debug.Log(spawn[0]);
        //TODO spawn all players along spawn objects
        var player = Instantiate(playerPrefab, spawn[0].transform);
        player.transform.parent = null;
        virtualCamera.Follow = player.transform;
    }
}
