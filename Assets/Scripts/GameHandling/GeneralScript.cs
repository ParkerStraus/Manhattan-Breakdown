using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GeneralScript : MonoBehaviour
{
    public static GeneralScript instance;
    public bool InUI;
    public Texture2D reticle;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        InUI = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!InUI)
        {
            Cursor.SetCursor(reticle, new Vector2(16,16), CursorMode.Auto);
        }
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
