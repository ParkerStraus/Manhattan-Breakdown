using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GeneralScript : MonoBehaviour
{
    public bool ResetFilters;
    public AudioMixer mixer;
    // Start is called before the first frame update
    void Start()
    {
        if (ResetFilters) GetComponent<MusicHandler>().TriggerFilter(22000f, 0.00f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene(1);
    }

}
