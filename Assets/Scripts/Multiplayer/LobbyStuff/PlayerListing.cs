using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    // Start is called before the first frame update
    public void SetPlayerInfo(Photon.Realtime.Player player)
    {
        text.text = player.NickName;
    }
}
