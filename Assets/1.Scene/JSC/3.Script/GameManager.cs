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
        yield return new WaitForSeconds(1f); // 임의의 딜레이를 줄 수 있습니다.

        // 서버에서 플레이어를 생성하고 초기화
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(player.GetComponent<NetworkIdentity>().connectionToClient, player);

        JoinPlayer joinPlayer = player.GetComponent<JoinPlayer>();
        if (joinPlayer != null)
        {
            // 플레이어의 이름 설정 (임의로 설정하거나 로그인 정보에서 가져와 설정 가능)
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
