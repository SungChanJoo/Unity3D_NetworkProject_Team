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

    //client�� server�� connect �Ǿ��� �� �ݹ��Լ�
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

    //Ŭ���̾�Ʈ�� Server�� ������ �� 
    [ClientCallback]
    private void OnDestroy()
    {
        if (!isLocalPlayer) return;
    }
    //RPC�� �ᱹ ClientRpc ��ɾ� < Command(server)��ɾ� < Client ��ɾ�?
    
    [Client]
    public void StartBtn()
    {
        if (isOwned)
        {
            CmdGameStart();
        }
        else
        {
            Debug.Log("���常 ������ �����ؿ�");
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
        Debug.Log(SceneName+"RPC ��ɾ� ����");
        SceneManager.LoadScene(SceneName);
        Debug.Log("�� �ε�!");

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
