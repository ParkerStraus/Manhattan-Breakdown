using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class debugGUI : MonoBehaviour
{
    [SerializeField] private UnityEvent NextSceneEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextScene()
    {
        NextSceneEvent.Invoke();
    }

    public void ConnectHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void ConnectClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
