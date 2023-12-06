using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class GameStartBtn : MonoBehaviour
{
    public string PlayStage;
    public void StartBtn()
    {
        
        NetworkRoomManager manager = GetComponent<NetworkRoomManager>();
        manager.AllPlayerSetReady();
        //manager.ServerChangeScene(PlayStage);
    }
}
