using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class GameStartBtn : MonoBehaviour
{
    [SerializeField] private RoomManager manager;
    public string PlayStage;
    public void StartBtn()
    {
        
        manager = GetComponent<RoomManager>();
        manager.AllPlayerSetReady();
        manager.ServerChangeScene(PlayStage);
    }
}
