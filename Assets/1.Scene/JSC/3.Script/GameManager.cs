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
    public int PlayerMaxCount = 8;
    public int PlayerNum;
    //[SerializeField] private InputField inputfield;
    [SerializeField] private GameObject cavas;
    [SerializeField] private GameObject startBtn;
    [SerializeField] private GameObject winnerUI_obj;
    //public GameObject[] PlayerList;
    public List<JoinPlayer> PlayerList = new List<JoinPlayer>();
    public bool isFirstPlayer = false;
    private int isAliveCount=0;

    WinnerUI winnerUI;
    private string GameWinner;
    private bool startGame = false;

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
        winnerUI = winnerUI_obj.GetComponent<WinnerUI>();
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
        isFirstPlayer = GameManager.Instance.PlayerNum == 0;

        while (true)
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

        if (startGame)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (!PlayerList[i].IsDead && isAliveCount < 2)
                {
                    isAliveCount++;
                    GameWinner = PlayerList[i].playerName;                    
                }               
                
            }
            if (isAliveCount == 1)
            {
                CmdWinnerUI(GameWinner);                
            }
            else
            {
                isAliveCount = 0;
            }
        }
    }
    //클라이언트가 Server를 나갔을 때 
    [ClientCallback]
    private void OnDestroy()
    {
        if (!isLocalPlayer) return;
    }

    //RPC는 결국 ClientRpc 명령어 < Command(server)명령어 < Client 명령어?

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
        if (isFirstPlayer && PlayerNum >= 2 )
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
        startGame = true;
    }

    
    private void CmdWinnerUI(string winner)
    {
        RPCWinnerUI(winner);
    }

    
    private void RPCWinnerUI(string winner)
    {
        winnerUI_obj.SetActive(true);
        winnerUI.WinnerUi(winner);
        startGame = false;
    }

}
