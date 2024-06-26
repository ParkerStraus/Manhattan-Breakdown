using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] 
    private string _gameVersion = "0.0.2";
    public string GameVersion {  get { return _gameVersion; } }
    [SerializeField] 
    private string _nickName = "NickNamebitch";
    public string Nickname 
    { 
        get 
        {
            if(PlayerPrefs.GetString("DisplayName") != "" && PlayerPrefs.GetString("DisplayName") != null)
            {
                return PlayerPrefs.GetString("DisplayName");
            }
            int value = Random.Range(0, 99999);
            return _nickName + value.ToString();
        } 
    }
}
