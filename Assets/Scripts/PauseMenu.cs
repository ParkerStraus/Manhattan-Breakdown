using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToGame()
    {
        gameObject.SetActive(false);
    }

    void ExitGame()
    {

    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

}
