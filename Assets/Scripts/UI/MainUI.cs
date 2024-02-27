using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Ammo;
    [SerializeField] private TMP_Text CountDown_Text;
    [SerializeField] private Animator Anim;
    [SerializeField] private bool DisplayAmmo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMainUI(int overrideShow, string[] weapon)
    {
        if (overrideShow == 2)
        {
            Anim.SetBool("ammo Reveal", false);
            Anim.CrossFade("ammo_unIdle", 0, 1);
            return;
        }
        //Toggle
        if (overrideShow == 0)
        {
            DisplayAmmo = false;
            Anim.SetBool("ammo Reveal", DisplayAmmo);
        }
        else if(overrideShow == 1)
        {
            DisplayAmmo = true;
            Anim.SetBool("ammo Reveal", DisplayAmmo);
            Title.text = weapon[0];
            Ammo.text = "AMMO: "+ weapon[1];

        }
    }
    public void CountDown(string Text)
    {
        Anim.Play("CountDown", 0, 0);
        CountDown_Text.text = Text;
    }
}
