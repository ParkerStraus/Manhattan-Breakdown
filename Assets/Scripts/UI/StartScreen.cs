using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System;

public class StartScreen : MonoBehaviourPunCallbacks
{

    public Animator anim;
    public Image[]  PlayerPortraits;
    public TMP_Text[] PlayerPortraitsTitles;
    public GameObject UI;
    public void StartGame()
    {
        Debug.Log(PlayerPortraitsTitles[0]);
        int i = 0;
        foreach (KeyValuePair<int, Photon.Realtime.Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            try
            {

                if (playerInfo.Value.IsInactive)
                {
                    continue;
                }
                else
                {
                    gameObject.transform.GetChild(1 + (i * 2)).GetComponent<TMP_Text>().text = playerInfo.Value.NickName;
                    gameObject.transform.GetChild(0 + (i * 2)).GetComponent<Image>().enabled = true;
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
            i++;
            if (i == 4) break;
        }
        while (i < 4)
        {
            gameObject.transform.GetChild(0 + (i * 2)).GetComponent<Image>().enabled = false;
            gameObject.transform.GetChild(1 + (i * 2)).GetComponent<TMP_Text>().text = "";
            i++;
        }
    }

    public void HideScreen()
    {
        gameObject.SetActive(false);
    }
}
