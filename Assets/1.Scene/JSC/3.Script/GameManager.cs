using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance = null;
    //[SerializeField] private Text chatText;
    [SerializeField] private Text playerCount;
    public int PlayerMaxCount = 2;
    public int PlayerNum;
    //[SerializeField] private InputField inputfield;
    [SerializeField] private GameObject cavas;
    [SerializeField] private GameObject startBtn;

    public readonly SyncList<JoinPlayer> PlayerList = new SyncList<JoinPlayer>();

    private bool isPlay = false;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private GameObject playerPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    
    public void AddPlayerOnServer(JoinPlayer playerInfo)
    {
        PlayerList.Add(playerInfo);
    }

    
    public void RemovePlayerOnServer(JoinPlayer playerInfo)
    {
        PlayerList.Remove(playerInfo);
        Debug.Log("���� �÷��̾� �̸� : "+playerInfo.playerName + " �� ���� �ο� �� "+PlayerList.Count);
    }
    private void Update()
    {
        UpdatePlayerNum();        
        
    }

    



    private bool IsGameInProgress()
    {
        return isPlay;
    }
    private bool IsPlayerCountOne()
    {
        return PlayerList.Count == 1;
    }


    //Ŭ���̾�Ʈ�� Server�� ������ �� 
    [ClientCallback]
    private void OnDestroy()
    {
        if (!isLocalPlayer) return;
    }
    //RPC�� �ᱹ ClientRpc ��ɾ� < Command(server)��ɾ� < Client ��ɾ�?

    public void UpdatePlayerNum()
    {
        if(PlayerList != null)
        {
            playerCount.text = $"{PlayerList.Count}/{PlayerMaxCount}";
            
        }
        else
        {
            Debug.Log("�÷��̾��Ʈ�� ������");
        }
    }
    void UpdateUI()
    {
        cavas.SetActive(false);
        //UI Logic to set the UI for the proper player
    }

    [Client]
    public void StartBtn()
    {
        if (PlayerList.Count>1)
        {
            Debug.Log(isOwned + "�����ض� ����");
            CmdGameStart();
        }
        else
        {
            Debug.Log("�ο����� ��á���");
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdGameStart()
    {
        RPCUpdateUI();
    }


    [ClientRpc]
    private void RPCUpdateUI()
    {
        UpdateUI();
        
    }
}
