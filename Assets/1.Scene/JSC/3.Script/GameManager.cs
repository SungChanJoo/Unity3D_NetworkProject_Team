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

    public static readonly List<JoinPlayer> PlayerList = new List<JoinPlayer>();


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
    private const string defaultPlayerName = "DefaultPlayer";

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(SpawnPlayers());
    }

    private IEnumerator SpawnPlayers()
    {
        yield return new WaitForSeconds(1f); // ������ �����̸� �� �� �ֽ��ϴ�.

        // �������� �÷��̾ �����ϰ� �ʱ�ȭ
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(player.GetComponent<NetworkIdentity>().connectionToClient, player);

        JoinPlayer joinPlayer = player.GetComponent<JoinPlayer>();
        if (joinPlayer != null)
        {
            // �÷��̾��� �̸� ���� (���Ƿ� �����ϰų� �α��� �������� ������ ���� ����)
            joinPlayer.CmdSetPlayerName("Player1");
        }
    }

    public void AddPlayerOnServer(JoinPlayer playerInfo)
    {
        PlayerList.Add(playerInfo);
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
