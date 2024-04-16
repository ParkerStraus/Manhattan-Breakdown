using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    private void Start()
    {
        
    }
    public Animator anim;
    public GameObject[] PlayerPortraits;
    public GameObject[] PlayerPortraitsTitles;
    public GameObject UI;

    public void HideScreen()
    {
        UI.SetActive(false);
    }
}
