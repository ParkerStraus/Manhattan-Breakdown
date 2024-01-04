using Cinemachine;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
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

    public void OnStartOfGame()
    {
        StartCoroutine(GoToNextArena());

    }

    public IEnumerator GoToNextArena()
    {
        string ArenaName  = arenaList.GetScene(-1);
        AsyncOperation scene = SceneManager.LoadSceneAsync(ArenaName, LoadSceneMode.Additive);

        while (!scene.isDone)
        {
            Debug.Log("loading new arena");
            yield return null;
        }
        PreparePlayersForArena();
        if(CurrentArena != null)
        {

        }
        CurrentArena = ArenaName;

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
