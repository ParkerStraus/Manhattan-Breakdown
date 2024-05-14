using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GeneralScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitGame()
    {
        StartCoroutine(Quit());
    }
    
    private IEnumerator Quit()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }

    public static void GotoMenu()
    {
        SceneManager.LoadScene("Menu");
        PhotonNetwork.LeaveRoom();
    }


    public static void GotoLobby()
    {
        SceneManager.LoadScene("Lobby");
        PhotonNetwork.LeaveRoom();
    }

}
