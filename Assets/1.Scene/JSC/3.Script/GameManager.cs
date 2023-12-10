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
        Debug.Log("나간 플레이어 이름 : "+playerInfo.playerName + " ㅣ 현재 인원 수 "+PlayerList.Count);
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


    //클라이언트가 Server를 나갔을 때 
    [ClientCallback]
    private void OnDestroy()
    {
        if (!isLocalPlayer) return;
    }
    //RPC는 결국 ClientRpc 명령어 < Command(server)명령어 < Client 명령어?

    public void UpdatePlayerNum()
    {
        if(PlayerList != null)
        {
            playerCount.text = $"{PlayerList.Count}/{PlayerMaxCount}";
            
        }
        else
        {
            Debug.Log("플레이어리스트가 널위한");
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
            Debug.Log(isOwned + "시작해라 제발");
            CmdGameStart();
        }
        else
        {
            Debug.Log("인원수가 안찼어요");
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
