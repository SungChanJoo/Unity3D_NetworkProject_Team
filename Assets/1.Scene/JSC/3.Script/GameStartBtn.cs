using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartBtn : MonoBehaviour
{
    public string PlayStage;
    public void StartBtn()
    {
        var manager = RoomManager.singleton;
        manager.ServerChangeScene(PlayStage); ;
    }
}
