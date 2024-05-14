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
using static UnityEngine.Rendering.DebugUI;
using Photon.Pun;


public class GameHandler : MonoBehaviourPunCallbacks, IGameHandler
{

    [Header("Pause Stuff")]

    [Header("Player Stuff")]
    public List<GameObject> players;
    public int PlayerAmt = 2;

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
    public GameObject FOVLight;

    [Header("UI")]
    public UnityEvent DisplayScoreBoard;
    public UnityEvent DisplayVHS;
    public GameObject StartScreen;
    public GameObject Scoreboard;
    public GameObject[] StartScreen_PlayerPortraits;
    public GameObject[] StartScreen_PlayerPortraitsTitles;
    public UnityEvent HideVHS;
    public GameObject FinalScoreBoard;
    public MainUI MainUI;

    public MusicHandler musicHand;
    [Header("Music/SoundStuff")]
    public AudioSource musicSource;
    public AudioSource aSource_extra;
    public AudioClip[] audioClips; 


    // Start is called before the first frame 
    void Start()
    {
        PhotonNetwork.OfflineMode = true;
        arenaList = Resources.Load<ArenaList>("ArenaList");
        //StartGame();
        PlayerAmt = 1 + DummyPlayers;

        for (int i = 0; i < PlayerAmt; i++)
        {
            points[i] = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {

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


    public void StartGame()
    {
        StartCoroutine(OnStartGame());
        playerObject.GetComponent<Player>().SetOffline();
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
        for (int i = 0; i < PlayerAmt; i++)
        {
            StartScreen_PlayerPortraits[i].SetActive(true);
            StartScreen_PlayerPortraitsTitles[i].SetActive(true);
        }
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
        aSource_extra.PlayOneShot(audioClips[0]);
        yield return new WaitForSeconds(1);
        MainUI.CountDown("2");
        aSource_extra.PlayOneShot(audioClips[0]);
        yield return new WaitForSeconds(1);
        MainUI.CountDown("1");
        aSource_extra.PlayOneShot(audioClips[0]);
        yield return new WaitForSeconds(1);
        CanPlayersDoStuff = true;
        MainUI.CountDown("Fight");
        aSource_extra.PlayOneShot(audioClips[1]);
        yield return new WaitForSeconds(1);

        //on end enable character control
        MainUI.CountDown("");
    }

    public void OnKill(int id)
    {
        Debug.Log("Kill Confirmed on "+id);
        playerAlive[id] = false;

        //Check if only one is alive
        int pALive = 0;
        for(int i = 0; i < PlayerAmt; i++)
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


    private IEnumerator NextRound()
    {
        Debug.Log("Next Round Starting");
        musicHand.TriggerSnapshot(1, 0.5f);

        yield return new WaitForSeconds(1);
        UpdateMainUI(2, null);
        CanPlayersDoStuff = false;
        DisplayVHS.Invoke();
        Scoreboard.SetActive(true);
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
        MainUI.Override();
        aSource_extra.PlayOneShot(audioClips[2]);
        CanPlayersDoStuff = false;
        DisplayVHS.Invoke();
        musicHand.TriggerSnapshot(1, 2f);

        yield return new WaitForSeconds(3);
        FinalScoreBoard.SetActive(true);
        string[] strings = { "1", "2", "3", "4" };
        FinalScoreBoard.GetComponent<FinalResults>().SetupScoreBoard(names: strings,scores: points);

    }

    public void OnRoundEnd()
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

        if (CurrentArena != "")
        {
            FOVLight.transform.SetParent(this.gameObject.transform);
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
        foreach(GameObject particle in particles)
        {
            Destroy(particle);
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
        player.GetComponent<Player>().SetOffline();
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

        //Add sight view
        FOVLight.transform.position = player.transform.position;
        FOVLight.transform.SetParent(player.transform);

        virtualCamera.gameObject.GetComponent<VirCamStuff>().SnapToFollow();
    }

    public int[] GetScore()
    {
        return points;
    }

    public int GetPlayerAmt() { return PlayerAmt; }

    public bool CanthePlayersMove()
    {
        return CanPlayersDoStuff;
    }


    public void UpdateMainUI(int UIOverride, string[] value)
    {
        MainUI.UpdateMainUI(UIOverride, value);
    }
}
