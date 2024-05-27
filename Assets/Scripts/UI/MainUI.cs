using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MainUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Ammo;
    [SerializeField] private TMP_Text CountDown_Text;
    [SerializeField] private Animator Anim;
    [SerializeField] private bool DisplayAmmo;
    [SerializeField] private bool FinalOverride = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMainUI(int setShow, string[] weapon)
    {
        //Toggle
        if (!FinalOverride)
        {
            if (setShow == 1)
            {
                DisplayAmmo = true;
                Anim.SetBool("ammo Reveal", DisplayAmmo);
                Title.text = weapon[0];
                if (weapon[1] == "-1") Ammo.text = "";
                else Ammo.text = "AMMO: " + weapon[1];
            }
            else if(setShow == 0)
            {
                DisplayAmmo = false;
                Anim.SetBool("ammo Reveal", DisplayAmmo);
            }
            if (setShow == 2)
            {
                DisplayAmmo = false;
                Anim.SetBool("ammo Reveal", DisplayAmmo);
                Anim.CrossFade("ammo_unIdle", 0, 1);
            }
        }
    }

    public void Override()
    {
        FinalOverride = true;
        Anim.SetBool("ammo Reveal", false);
        DisplayAmmo = false;
        Anim.CrossFade("ammo_unIdle", 0, 1);
        return;

    }
    public void CountDown(string Text)
    {
        Anim.Play("CountDown", 0, 0);
        CountDown_Text.text = Text;
    }
}
