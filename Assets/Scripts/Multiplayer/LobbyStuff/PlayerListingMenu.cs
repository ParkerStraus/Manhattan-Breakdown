using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerListing _playerListing;
    [SerializeField] private Transform _content;
    private List<PlayerListing> _listings = new List<PlayerListing>();
    private void Awake()
    {
        PlayerUpdate();
    }

    public void OnPlayerEnteredRoom()
    {
        PlayerUpdate();
    }

    public void OnPlayerLeftRoom()
    {
        PlayerUpdate();
    }

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
    }
}
