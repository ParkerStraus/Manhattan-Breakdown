using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    //[SerializeField] private PlayerListing _playerListing;
    [SerializeField] private Transform _content;
    [SerializeField] private TMP_Text[] PlayerNames;
    [SerializeField] private Image[] ProfilePic;

    private void Awake()
    {
        PlayerUpdate();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        PlayerUpdate();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        PlayerUpdate();
    }

    public void PlayerUpdate()
    {
        int i = 0;
        foreach (KeyValuePair<int, Photon.Realtime.Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            if (playerInfo.Value.IsInactive)
            {
                continue;
            }
            else
            {
                ProfilePic[i].enabled = true;
                PlayerNames[i].text = playerInfo.Value.NickName;
            }
            i++;
        }
        while (i < 4)
        {
            ProfilePic[i].enabled = false;
            PlayerNames[i].text = "";
            i++;
        }
    }


    /*
    public void PlayerUpdate()
    {
        foreach(KeyValuePair<int, Photon.Realtime.Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
    }

    private void AddPlayerListing(Photon.Realtime.Player player)
    {
        PlayerListing listing = Instantiate(_playerListing, _content);
        if(listing != null)
        {
            listing.SetPlayerInfo(player);
            _listings.Add(listing);
        }
    }*/
}
