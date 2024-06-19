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
    public PhotonView PV;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        PV = GetComponent<PhotonView>();
        instance = this;
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
    }

    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "Lobby") return;
        SceneManager.LoadScene("Lobby");
    }

    public void BackToLobby()
    {
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsVisible = true;
        PV.RPC("BackToLobbyRPC", RpcTarget.Others);
        SceneManager.LoadScene("Lobby");
    }

    [PunRPC]
    public void BackToLobbyRPC()
    {
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsVisible = true;
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
            photonView.RPC("ReturnToLobbyRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ReturnToLobbyRPC()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }

    #endregion


}
