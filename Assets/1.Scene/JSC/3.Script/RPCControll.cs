using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


public class RPCControll : NetworkBehaviour
{
    //[SerializeField] private Text chatText;
    [SerializeField] private Text playerCount;
    public int PlayerMaxCount = 2;
    [SyncVar] public int PlayerNum;
    //[SerializeField] private InputField inputfield;
    [SerializeField] private GameObject cavas;
    [SerializeField] private GameObject startBtn;
    public string SceneName;


    //private static event Action<string> onMessage;

    //client가 server에 connect 되었을 때 콜백함수
    public override void OnStartAuthority()
    {

        if (isLocalPlayer)
        {
            cavas.SetActive(true);
        }
        if (isOwned)
        {
            startBtn.SetActive(true);
        }
    }
    private void Update()
    {
        PlayerNum = NetworkClient.PlayerCount;
        playerCount.text = $"{PlayerNum}/{PlayerMaxCount}";
        //Debug.Log(NetworkClient.PlayerCount);
    }

    //클라이언트가 Server를 나갔을 때 
    [ClientCallback]
    private void OnDestroy()
    {
        if (!isLocalPlayer) return;
    }
    //RPC는 결국 ClientRpc 명령어 < Command(server)명령어 < Client 명령어?
    
    [Client]
    public void StartBtn()
    {
        if (isOwned)
        {
            CmdGameStart();
        }
        else
        {
            Debug.Log("방장만 시작이 가능해요");
        }
    }
    [Command]
    private void CmdGameStart()
    {
        RPCGamePlay();
    }
    [ClientRpc]
    private void RPCGamePlay()
    {
        Debug.Log(SceneName+"RPC 명령어 실행");
        SceneManager.LoadScene(SceneName);
        Debug.Log("씬 로드!");

    }
/*    [ClientRpc]
    private void RPCExitPlayer()
    {
        onPlayerManage?.Invoke();
    }*/
    /*    private void RPCHandlePlayerCount()
        {
            onJoinPlayer?.Invoke();
        }*/
}
