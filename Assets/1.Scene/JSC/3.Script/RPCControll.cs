using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class RPCControll : NetworkBehaviour
{
    public static RPCControll Instance = null;
    //[SerializeField] private Text chatText;
    [SerializeField] private Text playerCount;
    public int PlayerMaxCount = 2;
    public int PlayerNum;
    //[SerializeField] private InputField inputfield;
    [SerializeField] private GameObject cavas;
    [SerializeField] private GameObject startBtn;
    public List<JoinPlayer> PlayerList;


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


    public override void OnStartServer()
    {
        PlayerList = new List<JoinPlayer>();
        PlayerNum = 0;
    }

    //private static event Action<string> onMessage;
    private void Update()
    {
        UpdatePlayerNum();
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
        playerCount.text = $"{PlayerList.Count}/{PlayerMaxCount}";

       // playerCount.text = $"{PlayerNum}/{PlayerMaxCount}";
/*        if(PlayerList.Count != 0)
        {

        }
        else
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                Debug.Log(PlayerList[i].name + " | " + PlayerList[i].isFirst);

            }
        }*/
    }
    void UpdateUI()
    {
        cavas.SetActive(false);
        //UI Logic to set the UI for the proper player
    }

    [Client]
    public void StartBtn()
    {
        if (true)
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
