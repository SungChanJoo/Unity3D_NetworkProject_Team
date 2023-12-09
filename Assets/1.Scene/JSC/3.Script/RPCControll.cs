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

    public SyncList<PlayerInfo> PlayerList = new SyncList<PlayerInfo>();


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
    void Start()
    {
        // SyncList�� ����� �� ȣ��Ǵ� �ݹ� �Լ� ���
        PlayerList.Callback += OnPlayerListChanged;
    }
    private void OnPlayerListChanged(SyncList<PlayerInfo>.Operation op, int index, PlayerInfo oldItem, PlayerInfo newItem)
    {
        // ����� ���뿡 ���� ó��
        Debug.Log("PlayerList changed: " + op + " at index " + index);
    }
    /*    public override void OnStartServer()
        {
        }*/

    //private static event Action<string> onMessage;
    private void Update()
    {
        UpdatePlayerNum();
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
