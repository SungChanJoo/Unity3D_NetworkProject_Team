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

    //public GameObject[] PlayerList;
    public List<JoinPlayer> PlayerList = new List<JoinPlayer>();


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
        base.OnStartServer();
    }
    
    /*    public void AddPlayerOnServer(JoinPlayer playerInfo)
        {
            PlayerList.Add(playerInfo);
        }*/
    IEnumerator UpdateList_co()
    {
        while(true)
        {
            if (NetworkClient.active)
            {
                PlayerList.Clear();
                GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < playerArray.Length; i++)
                {
                    if (PlayerList.Count < playerArray.Length)
                    {
                        PlayerList.Add(playerArray[i].GetComponent<JoinPlayer>());
                    }
                    else
                    {
                        PlayerList[i] = playerArray[i].GetComponent<JoinPlayer>();
                    }
                }
                PlayerNum = PlayerList.Count;
            }
            yield return null;
        }
    }
    private void Start()
    {
        StartCoroutine(UpdateList_co());
    }
    private void Update()
    {
        UpdatePlayerNumUI();
    }
    //Ŭ���̾�Ʈ�� Server�� ������ �� 
    [ClientCallback]
    private void OnDestroy()
    {
        Debug.Log("Ŭ���̾�Ʈ �����߾�");
        if (!isLocalPlayer) return;
    }

    //RPC�� �ᱹ ClientRpc ��ɾ� < Command(server)��ɾ� < Client ��ɾ�?

    public void UpdatePlayerNumUI()
    {
        playerCount.text = $"{PlayerNum}/{PlayerMaxCount}";
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
}
