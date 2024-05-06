using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyHandler : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject CreateOrJoinLobby;
    [SerializeField]
    private GameObject CurrentRoomLobby;
    [SerializeField]
    private GameObject StartGameButton;

    [SerializeField]
    private TMP_Text RoomTitle;

    void SetInRoom(bool inRoom)
    {
        if(inRoom)
        {
            CreateOrJoinLobby.SetActive(false);
            CurrentRoomLobby.SetActive(true);
        }
        else
        {
            CreateOrJoinLobby.SetActive(true);
            CurrentRoomLobby.SetActive(false);
        }
    }

    public override void OnJoinedRoom()
    {
        SetInRoom(true);
        RoomTitle.text = PhotonNetwork.CurrentRoom.Name;
            print(PhotonNetwork.PlayerList);

        //Disable start game if not host
        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.SetActive(true);
        }
        else
        {
            StartGameButton.SetActive(false);
        }
        base.OnJoinedRoom();
    }

    public override void OnLeftRoom()
    {
        SetInRoom(false);
        base.OnLeftRoom();
    }

    public void OnClick_DisconnectRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGame()
    {
        Debug.Log("Going to game now");
        PhotonNetwork.LoadLevel(3);
    }

}
