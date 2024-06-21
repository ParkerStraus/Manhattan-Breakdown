using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    #region Connection Stuff

    public static RoomManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 3)
        {

            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        if(scene.buildIndex == 2)
        {

            OnlineGameCoordinator.instance.GamePrep();
        }
    }

    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "Lobby") return;
        DeleteAllOBj();
        SceneManager.LoadScene("Lobby");
    }

    public void BackToLobby()
    {
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsVisible = true;
        photonView.RPC("BackToLobbyRPC", RpcTarget.Others);
        DeleteAllOBj();
        SceneManager.LoadScene("Lobby");
    }

    [PunRPC]
    public void BackToLobbyRPC()
    {
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsVisible = true;
        DeleteAllOBj();
        SceneManager.LoadScene("Lobby");
    }

    #endregion

    #region Player Disconnection Handling

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player " + otherPlayer.NickName + " has left the room.");
        CheckRemainingPlayers();
    }

    private void CheckRemainingPlayers()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            Debug.Log("Only one player remaining or all players disconnected. Bringing everyone back to the lobby.");
            photonView.RPC("DisconnectToLobbyRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void DisconnectToLobbyRPC()
    {
        if (SceneManager.GetActiveScene().name == "Lobby") return;
        PhotonNetwork.LeaveRoom();
        DeleteAllOBj();
        SceneManager.LoadScene("Lobby");
    }

    public void DeleteAllOBj()
    {
        GameElement[] elements = GameObject.FindObjectsOfType<GameElement>();
        PlayerManager[] pm = GameObject.FindObjectsOfType<PlayerManager>();
        foreach(GameElement p in elements)
        {
            Destroy(p.gameObject);
        }
        foreach(PlayerManager player in pm)
        {
            Destroy(player.gameObject);
        }
    }

    #endregion
}
