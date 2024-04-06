using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;
    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _text.text = roomInfo.PlayerCount +"/" + roomInfo.MaxPlayers + " - " + roomInfo.Name;
    }

    public void OnClick_JoinRoom()
    {
        //Join Game
        PhotonNetwork.JoinRoom(RoomInfo.Name);
        print(PhotonNetwork.PlayerList);
    }
}
