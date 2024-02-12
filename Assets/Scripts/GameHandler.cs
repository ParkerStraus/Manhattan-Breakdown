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
    public bool CanPlayersDoStuff;

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
    public Vector3 OffsetCurrent = Vector3.zero;
    public float OffsetInterp;
    public float OffsetClamp;
    public GameObject playerPrefab;
    public GameObject playerObject;

    [Header("UI")]
    public UnityEvent DisplayScoreBoard;
    public UnityEvent DisplayVHS;
    public GameObject StartScreen;
    public UnityEvent HideVHS;
    public GameObject FinalScoreBoard;
    public MainUI MainUI;

    [Header("Music/SoundStuff")]
    public MusicHandler musicHand;
    public AudioSource aSource;
    public AudioClip[] audioClips; 


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

        //Edit Position of camera via aim
        Vector2 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(playerObject != null)
        {
            Vector2 offset = aimPos - (Vector2)playerObject.transform.position;
            //Debug.Log(aimPos + ", " + Vector3.Magnitude(offset));
            Vector3 OffsetRealized = new Vector3(Mathf.Clamp(offset.x / 4, -OffsetClamp, OffsetClamp), Mathf.Clamp(offset.y / 4, -OffsetClamp, OffsetClamp), -5);
            OffsetCurrent = Vector3.Lerp(OffsetCurrent, OffsetRealized, OffsetInterp*Time.deltaTime);
            virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = OffsetCurrent;
        }
    }

    public void OnGamePause()
    {
        CurrentlyPaused = !CurrentlyPaused;
        if(CurrentlyPaused)
        {
            musicHand.TriggerSnapshot(1, 0.01f);
            OnPause.Invoke();
        }
        else
        {
            musicHand.TriggerSnapshot(0, 0.01f);
            OnUnpause.Invoke();
        }
    }
    public bool IsPaused()
    {
        return CurrentlyPaused;
    }

    public void StartGame()
    {
        StartCoroutine(OnStartGame());
    }

    private IEnumerator OnStartGame()
    {
        //Start Animation of players
        musicHand.TriggerSnapshot(1, 0.01f);
        Debug.Log("Now starting game");
        //Load Level
        GetMapReadyServerRPC();
        CanPlayersDoStuff = false;
        StartScreen.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        //Start Song
        musicHand.PlayRandomSong();
        yield return new WaitForSeconds(6.8f);
        musicHand.TriggerSnapshot(0, 3f);
        //disable ui
        StartScreen.GetComponent<Animator>().SetTrigger("Finish");
        yield return new WaitForSeconds(0.5f);
        musicHand.ShowSongInfo();
        yield return new WaitForSeconds(1.5f);
        StartScreen.SetActive(false);

        //Start countdown
        
        MainUI.CountDown("3");
        aSource.PlayOneShot(audioClips[0]);
        yield return new WaitForSeconds(1);
        MainUI.CountDown("2");
        aSource.PlayOneShot(audioClips[0]);
        yield return new WaitForSeconds(1);
        MainUI.CountDown("1");
        aSource.PlayOneShot(audioClips[0]);
        yield return new WaitForSeconds(1);
        MainUI.CountDown("Fight");
        aSource.PlayOneShot(audioClips[1]);

        //on end enable character control
        CanPlayersDoStuff = true;
        yield return new WaitForEndOfFrame();
        MainUI.CountDown("");
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
        musicHand.TriggerSnapshot(1, 0.5f);

        yield return new WaitForSeconds(1);
        CanPlayersDoStuff = false;
        DisplayVHS.Invoke();
        DisplayScoreBoard.Invoke();
        yield return new WaitForSeconds(2.25f);

        musicHand.TriggerSnapshot(0, 0.5f);
        yield return new WaitForSeconds(0.50f);
        HideVHS.Invoke();


        OnRoundEnd();
        CanPlayersDoStuff = true;

    }



    private IEnumerator GameComplete()
    {
        CanPlayersDoStuff = false;
        Debug.Log("Next Round Starting");
        DisplayVHS.Invoke();
        musicHand.TriggerSnapshot(1, 2f);

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
        player.transform.SetParent(null);
        playerAlive[0] = true;
        virtualCamera.Follow = player.transform;
        playerObject = player;


        for(int i = 1; i <= DummyPlayers; i++)
        {
            playerAlive[i] = true;
            var dummyPlayer = Instantiate(dummyPrefab, spawn[int.Parse(shuffledLoadOrder[i].ToString())].transform);
            dummyPlayer.GetComponent<DummyPlayer>().PID = i;   
            dummyPlayer.transform.parent = null;
        }


        virtualCamera.gameObject.GetComponent<VirCamStuff>().SnapToFollow();
    }

    public int[] GetScore()
    {
        return points;
    }

    public bool CanthePlayersMove()
    {
        return CanPlayersDoStuff;
    }

    public void UpdateMainUI(string[] Weapon)
    {
        MainUI.UpdateMainUI(Weapon);
    }
}
