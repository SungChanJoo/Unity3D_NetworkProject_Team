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
    [SerializeField] private GameObject aliveUI_obj;
    public List<JoinPlayer> PlayerList = new List<JoinPlayer>();

    

    public bool isFirstPlayer = false;
    private int isAliveCount = 0;
    WinnerUI winnerUI;
    private string GameWinner;
    private bool startGame = false;
    private int isAliveCountUI;

    public GameObject SafeZone;
    [SerializeField] private int SafeZoneSpawnTime = 60;
    public Text SafeTimeText;
    [SerializeField] private Transform[] spawnPos;
    private List<int> spawnPosList;

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
            isAliveCountUI = 0;
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (!PlayerList[i].IsDead && isAliveCount < 2)
                {                    
                    GameWinner = PlayerList[i].playerName;
                    isAliveCount++;
                }

                if (!PlayerList[i].IsDead)
                {
                    isAliveCountUI++;
                }               
            }

            AliveUI.Instance.IsAliveUI(isAliveCountUI);
            
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
        StartCoroutine(SpawnSafeZone_co());
    }

    [ClientRpc]
    private void RPCUpdateUI()
    {
        UpdateUI();
        startGame = true;
        aliveUI_obj.SetActive(startGame);

        spawnPosList = new List<int>();
        for (int i = 0; i < PlayerList.Count; i++)
        {
            spawnPosList.Add(i);
        }
        int j = 0;
        while(spawnPosList.Count > 0)
        {
            
            int rand = Random.Range(0, spawnPosList.Count); // 0, 1
            int spawnPosIndexValue = spawnPosList[rand]; // spawnPosList[1] = 1
            spawnPosList.RemoveAt(rand);
            PlayerList[j].gameObject.transform.position = spawnPos[spawnPosIndexValue].transform.position;
            j++;
        }
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

    [Server]
    IEnumerator SpawnSafeZone_co()
    {
        Debug.Log("is startGame ? :" + startGame);
        while(true)
        {
            int rand = Random.Range(0, spawnPos.Length);
            GameObject obj = Instantiate(SafeZone, spawnPos[rand].position, Quaternion.identity);
            NetworkServer.Spawn(obj);

            for (int i = 0; i < SafeZoneSpawnTime; i++) //60초후에
            {
                string time = $"{SafeZoneSpawnTime - i}";
                RpcSafeZoneTime(time);
                yield return new WaitForSeconds(1f);
            }
            RpcKillPlayer();
            RpcSafeZoneTime("");
            NetworkServer.Destroy(obj);
            yield return new WaitForSeconds(5f);
        }
    }
    [ClientRpc]
    public void RpcSafeZoneTime(string time)
    {
        SafeTimeText.text = time;
    }
    [ClientRpc]
    public void RpcKillPlayer()
    {
        Debug.Log("PlayerList.Count :" + PlayerList.Count);
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerMove player = PlayerList[i].gameObject.GetComponent<PlayerMove>();
            if (!player.IsSafe && !player.isDie)
            {
                Debug.Log($"Player : {PlayerList[i].playerName} IsSafe? : {player.IsSafe}");
                player.Die("DeadZone", PlayerList[i].playerName);
            }
        }
    }
}