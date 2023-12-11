using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance = null;
    [SerializeField] private Text playerCount;
    public int PlayerMaxCount = 8;
    public int PlayerNum;

    [SerializeField] private GameObject cavas;
    [SerializeField] private GameObject startBtn;
    [SerializeField] private GameObject winnerUI_obj;
    public List<JoinPlayer> PlayerList = new List<JoinPlayer>();

    

    public bool isFirstPlayer = false;
    private int isAliveCount = 0;
    WinnerUI winnerUI;
    private string GameWinner;
    private bool startGame = false;
    private void Awake()
    {
        if (Instance == null)
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
                CreateRoomUI.Instance.UpdatePlayerImage(PlayerNum);
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
    [ClientCallback]
    private void OnDestroy()
    {
        if (!isLocalPlayer) return;
    }
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

        if (isFirstPlayer && PlayerNum >= 2)
        {
            CmdGameStart();
        }
        else
        {
            Debug.Log("인원이 아직 안찼습니다.");
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