using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;

public class CreateRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _roomName;

    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, option, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        print("Created room successfully");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("room creation failed "+ message);
        base.OnCreateRoomFailed(returnCode, message);
    }
}
