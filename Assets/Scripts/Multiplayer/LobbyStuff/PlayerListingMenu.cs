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

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            LobbyHandler.Instance.EnableStartButton(true);
        }
        else
        {

            LobbyHandler.Instance.EnableStartButton(false);
        }
    }

    public void PlayerUpdate()
    {
        // Create a list to hold the players sorted by ActorNumber
        List<Photon.Realtime.Player> sortedPlayers = new List<Photon.Realtime.Player>();

        // Add active players to the list
        foreach (KeyValuePair<int, Photon.Realtime.Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            if (!playerInfo.Value.IsInactive)
            {
                sortedPlayers.Add(playerInfo.Value);
            }
        }

        // Sort the list by ActorNumber
        sortedPlayers.Sort((player1, player2) => player1.ActorNumber.CompareTo(player2.ActorNumber));

        // Display the player names in sorted order
        int i = 0;
        foreach (Photon.Realtime.Player player in sortedPlayers)
        {
            ProfilePic[i].enabled = true;
            PlayerNames[i].text = player.NickName;
            i++;
        }

        // Disable remaining slots if any
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
