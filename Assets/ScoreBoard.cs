using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private GameHandler _gh;
    [SerializeField] private TMP_Text[] Label;
    [SerializeField] private TMP_Text[] Scores;
    [SerializeField] private AudioClip ding;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void InitScoreBoard()
    {
        int[] score = _gh.GetScore();
        for (int i = 3; i > _gh.playerAmt; i--)
        {
                Label[i].text = "";
                Scores[i].text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreboard()
    {
        AudioSource.PlayClipAtPoint(ding,Camera.main.transform.position);

        int[] score = _gh.GetScore();

        for (int i = 0; i < score.Length; i++)
        {
            Debug.Log(score[i]);
            if (score[i] == -1)
            {
                Label[i].text = "";
                Scores[i].text = "";
            }
            else
            {
                Scores[i].text = (score[i]).ToString();
            }

        }
    }

    public void TurnOff()
    {
        this.gameObject.SetActive(false);
    }
}
