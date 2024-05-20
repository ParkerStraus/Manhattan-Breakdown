using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;


public class ScoreBoard : MonoBehaviour
{
    [SerializeField, SerializeReference]
    public Animator anim;
    public IGameHandler _gh;
    [SerializeField] private TMP_Text[] Label;
    [SerializeField] private TMP_Text[] Scores;
    [SerializeField] private AudioClip ding;
    [SerializeField] private AudioSource ac;
    // Start is called before the first frame update
    void Start()
    {
        try
        {

        if(_gh == null)
        {
            _gh = GameObject.Find("Main Camera").GetComponent<IGameHandler>();
            InitScoreBoard();
            
        }
        }
        catch { Debug.LogWarning("Main Camera not found"); }
        
    }

    public void SetGH(IGameHandler gh)
    {
        _gh = gh;
        InitScoreBoard();
    }

    public void InitScoreBoard()
    {
        int[] score = _gh.GetScore();
        string[] players = _gh.GetPlayerNames();
        for (int i = 3; i > _gh.GetPlayerAmt() - 1; i--)
        {
            Label[i].text = "";
            Scores[i].text = "";
        }


        for (int i = 0; i > _gh.GetPlayerAmt() - 1; i++)
        {
            Label[i].text = players[i];
            Scores[i].text = score[i].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnim()
    {
        anim.SetTrigger("go");
    }

    public void UpdateScoreboard()
    {
        ac.PlayOneShot(ding);
        
        int[] score = _gh.GetScore();

        Debug.Log(String.Join(", ", score));

        for (int i = 0; i < _gh.GetPlayerAmt(); i++)
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
