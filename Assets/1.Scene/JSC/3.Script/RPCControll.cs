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
    public int PlayerNum;
    //[SerializeField] private InputField inputfield;
    [SerializeField] private GameObject cavas;
    [SerializeField] private GameObject startBtn;
    public string SceneName;

    public override void OnStartServer()
    {
        PlayerNum = 0;
    }
    //private static event Action<string> onMessage;
    private void Update()
    {
        PlayerNum = GameObject.FindGameObjectsWithTag("Player").Length;
        UpdatePlayerNum();
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
        if (PlayerNum == PlayerMaxCount)
        {

            Debug.Log(isOwned + "�����ض� ����");
            CmdGameStart();
        }
        else
        {
            Debug.Log("�ο����� ��á���");
        }
    }
    void UpdateUI()
    {
        cavas.SetActive(false);
        //UI Logic to set the UI for the proper player
    }

    [Command(requiresAuthority = false)]
    private void CmdGameStart()
    {
        RPCUpdateUI();
        //RPCStartGame();
    }
    public void UpdatePlayerNum()
    {
        playerCount.text = $"{PlayerNum}/{PlayerMaxCount}";
    }

    [ClientRpc]
    private void RPCUpdateUI()
    {
        UpdateUI();
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
