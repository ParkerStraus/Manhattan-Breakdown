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

public class PlayerManager : MonoBehaviourPunCallbacks, IGameHandler
{
    PhotonView PV;
    public int PlayerAmt { get; set; }
    public int[] score;
    public bool CanThePlayerDoStuff = true;

    [Header("UI")]
    public MainUI MainUI;
    public ScoreBoard _ScoreBoard;

    [Space(20)]
    [Header("Music/SoundStuff")]
    public MusicHandler musicHand;
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
            //Lock Virtual Camera to Player
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

    #region Next Round

    public void NextRound(int[] points)
    {
        if (PV.IsMine)
        {
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
        CanThePlayerDoStuff = false;
        Debug.Log(String.Join(", ", score));
        _ScoreBoard.PlayAnim();
        yield return new WaitForSeconds(0.1f);
        Player[] players = GameObject.FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }
        RoomManager.instance.PlayerNowInTransition(PhotonNetwork.LocalPlayer.ActorNumber - 1);
        //Send message to Room Manager to start loading new level
        yield return new WaitForSeconds(0.9f);
        _ScoreBoard.UpdateScoreboard();

        yield return new WaitForSeconds(2f);

        musicHand.TriggerSnapshot(0, 0.5f);
        yield return new WaitForSeconds(0.50f);
        CanThePlayerDoStuff = true;
    }

    #endregion

    #region Game Complete

    #endregion

}
