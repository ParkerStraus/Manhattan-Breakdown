using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Unity.Netcode;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

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
        RoomManager.instance.RegisterPlayerManager(this);
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            MainUI = GameObject.Find("MainUI").GetComponent<MainUI>();
            _ScoreBoard = GameObject.Find("UI").GetComponent<ScoreBoard>();
            _ScoreBoard.SetGH(this);
            musicHand = GameObject.Find("Main Camera").GetComponent<MusicHandler>();
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

    #region Next Round

    public void NextRound()
    {
        PV.RPC("NextRoundVisualRPC", RpcTarget.All);
    }

    [PunRPC]
    public void NextRoundVisualRPC()
    {
        if (PV.IsMine)
        {
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
        _ScoreBoard.PlayAnim();
        yield return new WaitForSeconds(0.25f);
        //Send message to Room Manager to start loading new level
        yield return new WaitForSeconds(0.75f);
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
