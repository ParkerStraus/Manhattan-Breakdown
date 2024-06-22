using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] bool Paused = false;
    public GameObject UI;
    public UnityEvent OnPause;
    public UnityEvent OnUnpause;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnGamePause();
        }
    }

    public void OnGamePause()
    {
        Paused = !Paused;
        if (Paused)
        {
            OnPause.Invoke();
        }
        else
        {
            OnUnpause.Invoke();
        }
    }
    public bool IsPaused()
    {
        return Paused;
    }

    public void ReturnToGame()
    {
        gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        RoomManager.instance.BackToLobby();
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

}
