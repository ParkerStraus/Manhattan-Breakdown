using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Unity.VisualScripting;

public class FinalResults : MonoBehaviourPunCallbacks
{
    public GameObject scoreboard;
    [SerializeField] private TMP_Text[] Label;
    [SerializeField] private TMP_Text[] Scores;
    public GameObject MasterStuff;
    public GameObject NotMasterStuff;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToLobby()
    {
        FindObjectOfType<RoomManager>().BackToLobby();
    }
    public void RestartGame()
    {
        FindObjectOfType<OnlineGameCoordinator>().RestartGame();
    }


    public void SetupScoreBoard(string[] names, int[] scores)
    {
        scoreboard.SetActive(true);
        int[][] rankedScores = new int[scores.Length][];

        for (int i = 0; i < scores.Length; i++)
        {
            rankedScores[i] = new int[] { i, scores[i] };
        }

        rankedScores = rankedScores.OrderByDescending(x => x[1]).ToArray();
        for(int i = 0; i < rankedScores.Length; i++)
        {

            if (rankedScores[i][1] == -1)
            {
                Label[i].text = "";
                Scores[i].text = "";
            }
            else
            {
                Label[i].text = names[rankedScores[i][0]];
                Scores[i].text = (rankedScores[i][1]).ToString();
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            MasterStuff.SetActive(true);
            NotMasterStuff.SetActive(false);
        }
        else
        {
            MasterStuff.SetActive(false);
            NotMasterStuff.SetActive(true);
        }
    }
}
