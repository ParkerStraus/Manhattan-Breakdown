using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RoomListing _roomListing;
    [SerializeField]
    private Transform _content;

    private List<RoomListing> _listings = new List<RoomListing>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    { 
        foreach (RoomInfo roomInfo in roomList)
        {
            if(roomInfo.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == roomInfo.Name);
                if(index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
            else
            {
                RoomListing listing = Instantiate(_roomListing, _content);
                if (listing != null)
                {
                    listing.SetRoomInfo(roomInfo);
                    _listings.Add(listing);
                }

            }
        }
    }
}
