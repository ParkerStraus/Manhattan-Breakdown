using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Unity.Netcode;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using System;
using Unity.Mathematics;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviourPunCallbacks, IGameHandler
{
    PhotonView PV;
    public int PlayerAmt { get; set; }
    public int[] score;
    public bool CanThePlayerDoStuff = true;
    public bool CanIReturnToGame = true;

    [Header("UI")]
    public MainUI MainUI;
    public ScoreBoard _ScoreBoard;
    public FinalResults FinalScoreBoard;
    public StartScreen StartScreen;
    public VHS VHS;

    [Header("Music/SoundStuff")]
    public MusicHandler musicHand;
    public AudioSource musicSource;
    public AudioSource aSource_extra;
    public AudioClip[] audioClips;

    // Start is called before the first frame update
    void Start()
    {
        PlayerAmt = PhotonNetwork.PlayerList.Length;
        RoomManager.instance.RegisterPlayerManager(this);
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            MainUI = GameObject.Find("MainUI").GetComponent<MainUI>();
            _ScoreBoard = GameObject.Find("UI").GetComponent<ScoreBoard>();
            _ScoreBoard.SetGH(this);
            musicHand = GameObject.Find("Main Camera").GetComponent<MusicHandler>();
            FinalScoreBoard = GameObject.Find("UI").GetComponent<FinalResults>();
            //Lock Virtual Camera to Player
            aSource_extra = GameObject.Find("ExtraSoundEffects").GetComponent<AudioSource>();
            VHS = GameObject.Find("Main Camera").GetComponent<VHS>();
            StartScreen = GameObject.Find("UI").GetComponent<StartScreen>();
        }
        
    }

    public bool IsMine()
    {
        return PV.IsMine;
    }

    public void SpawnPlayer(Vector3 pos)
    {
        PV = GetComponent<PhotonView>();
        PV.RPC("SpawnPlayerLocal", RpcTarget.All, pos);
    }


    public void OnKill(int whoDied)
    {
        if(PV.IsMine)
        {
            Debug.Log("Person has died sending to Photon");
            RoomManager.instance.PlayerDied(PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else{
            Debug.Log("Other Person has died");
        }
    }

    [PunRPC]
    public void SpawnPlayerLocal(Vector3 pos)
    {

        if (PV.IsMine)
        {
            var pla = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), pos, Quaternion.identity);

            pla.GetComponent<Player>().SetIGH(this);
            GameObject.Find("VirCam").GetComponent<VirCamStuff>().SetNewObject(pla);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Is Master Client");
            }
        }
    }

    public bool CanthePlayersMove()
    {
        return CanThePlayerDoStuff;
    }

    public void UpdateMainUI(int UIOverride, string[] value)
    {
        MainUI.UpdateMainUI(UIOverride, value);
    }

    public int[] GetScore()
    {
        return score;
    }

    public int GetPlayerAmt()
    {
        return PlayerAmt;
    }

    #region Start Game
    
    public void StartGame_phase1()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine) return;
        musicHand.PlayRandomSong();
        StartScreen.anim.SetTrigger("Start");
        //Start Animation of players
        musicHand.TriggerSnapshot(1, 0.01f);
        Debug.Log("Now starting game");
        //Load Level
        CanThePlayerDoStuff = false;
        //StartScreen.SetActive(true);
        for (int i = 0; i < PlayerAmt; i++)
        {
            StartScreen.PlayerPortraits[i].SetActive(true);
            StartScreen.PlayerPortraitsTitles[i].SetActive(true);
        }
    }


    public void StartGame_phase2()
    {
        if (!PV.IsMine) return;
        StartCoroutine(StartGame_phase2Routine());
    }

    public IEnumerator StartGame_phase2Routine()
    {
        StartScreen.anim.SetTrigger("Finish");
        musicHand.TriggerSnapshot(0, 1f);
        yield return new WaitForSeconds(0.5f);
        musicHand.ShowSongInfo();
        yield return new WaitForSeconds(2f + (27f/60f));
        StartScreen.HideScreen();
    }

    public void StringCountdown(string String, int i)
    {
        if (PV.IsMine)
        {
            MainUI.CountDown(String);
            aSource_extra.PlayOneShot(audioClips[i]);
        }
    }


    public void StartGame_phase3()
    {

        if (!PV.IsMine) return;
        StartCoroutine(CommenceGameStart());
    }

    public IEnumerator CommenceGameStart()
    {
        CanThePlayerDoStuff = true;
        MainUI.CountDown("Fight");
        aSource_extra.PlayOneShot(audioClips[1]);
        yield return new WaitForSeconds(1);

        //on end enable character control
        MainUI.CountDown("");
    }

    #endregion

    #region Next Round

    public void NextRound(int[] points)
    {
        if (PV.IsMine)
        {
            CanIReturnToGame = false;
            Debug.Log(points);
            score = points;
            StartCoroutine(NextRoundStuff());
        }
    }

    public IEnumerator NextRoundStuff()
    {
        Debug.Log("Next Round Starting");
        musicHand.TriggerSnapshot(1, 0.5f);

        yield return new WaitForSeconds(1);
        UpdateMainUI(2, null);
        VHS.DisplayVHS();
        CanThePlayerDoStuff = false;
        Debug.Log(String.Join(", ", score));
        _ScoreBoard.PlayAnim();
        yield return new WaitForSeconds(0.1f);

        //Delete Leftover Players
        Player[] players = GameObject.FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }

        //Delete leftover Gun Objects
        WeaponPickup[] guns = GameObject.FindObjectsOfType<WeaponPickup>();
        for (int i = 0; i < guns.Length; i++)
        {
            Destroy(guns[i].gameObject);
        }

        RoomManager.instance.PlayerNowInTransition(PhotonNetwork.LocalPlayer.ActorNumber - 1);
        //Send message to Room Manager to start loading new level
        yield return new WaitForSeconds(0.9f);
        _ScoreBoard.UpdateScoreboard();

        yield return new WaitForSeconds(2f);
        musicHand.TriggerSnapshot(0, 0.5f);
        while (true)
        {
            if (CanIReturnToGame)
            {
                VHS.HideVHS();
                CanThePlayerDoStuff = true;
                break;
            }

            yield return new WaitForEndOfFrame();
        }

    }

    public void NextRoundNowLoaded()
    {
        PV.RPC("NextRoundNowLoadedRPC",RpcTarget.All);
    }

    [PunRPC]
    public void NextRoundNowLoadedRPC()
    {
        if (PV.IsMine)
        {
            CanIReturnToGame = true;
        }
    }

    #endregion

    #region Game Complete

    public void GameComplete(int[] points)
    {
        score = points;
        if(PV.IsMine)
        {
            StartCoroutine(GameCompleteRoutine());
        }
    }

    private IEnumerator GameCompleteRoutine()
    {

        //Blank Main UI
        MainUI.Override();
        //Play special sound effect
        aSource_extra.PlayOneShot(audioClips[2]);
        //Disable move for player
        CanThePlayerDoStuff = false;
        //Fade out music
        musicHand.TriggerSnapshot(1, 2f);

        yield return new WaitForSeconds(3);
        //Set Final Scoreboard and display
        FinalScoreBoard.SetupScoreBoard(scores: score);

    }

    #endregion

}
