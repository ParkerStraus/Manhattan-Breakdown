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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMainUI(string[] weapon)
    {
        Title.text = weapon[0];
        Ammo.text = weapon[1];
    }
    public void CountDown(string Text)
    {
        Anim.Play("CountDown", 0, 0);
        CountDown_Text.text = Text;
    }
}
