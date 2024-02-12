using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FinalResults : MonoBehaviour
{
    [SerializeField] private TMP_Text[] Label;
    [SerializeField] private TMP_Text[] Scores;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupScoreBoard(int[] scores)
    {
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
                Label[i].text = "Player " + rankedScores[i][0];
                Scores[i].text = (rankedScores[i][1]).ToString();
            }
        }
    }
}
