using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class TestPlayerMove : NetworkBehaviour
{
    [Header("healthBar")]
    public TextMesh healthBar;

    [SerializeField] private GameObject player;
    [SerializeField] private float WalkSpeed = 10f;
    [SerializeField] private float RunSpeed = 15f;
    [SerializeField] private float yVelocity = 0;
    [SerializeField] private float xVelocity = 0;
    [SerializeField] private float RotationSpeed = 3f;


    [Header("Attack_Colider")]
    [SerializeField] private Collider attack_col;

    [SerializeField] private Animator anim;

    private float Velocity;

    private bool iswalk = false;
    private bool isrun = false;
    private bool isAttack = false;

    [Header("Att_cool")]
    [SerializeField] private float Attack_Cool = 0f;

    [SyncVar] public int health = 1;
    [SyncVar] public string playerName;

    [SyncVar] bool die = false;
    private int AttacktCount = 0;

    //--------------------------UI--------------------------
    
    public event System.Action<byte> OnPlayerNumberChanged;
    public event System.Action<Color32> OnPlayerColorChanged;
    public event System.Action<ushort> OnPlayerDataChanged;
    


    static readonly List<TestPlayerMove> playersList = new List<TestPlayerMove>();

    [Header("Player UI")]
    public GameObject playerUIPrefab;

    GameObject playerUIObject;
    PlayerTestUI playerUI = null;

    #region SyncVars

    [Header("SyncVars")]

    //각 동기화 걸어놓기
    [SyncVar(hook = nameof(PlayerNumberChanged))]
    public byte playerNumber = 0;
    
    [SyncVar(hook = nameof(PlayerColorChanged))]
    public Color32 playerColor = Color.white;
    
    [SyncVar(hook = nameof(PlayerDataChanged))]
    public ushort playerData = 0;
    


    //--------------------------킬로그--------------------------

    [Header("Player UI")]
    public GameObject killLogUIPrefab;

    GameObject KillLogUIObject;
    KillLogUi killLogUi = null;

    private void Awake()
    {
        TryGetComponent(out anim);        
    }

    void Update()
    {
        healthBar.text = new string('-', health);
        if (isLocalPlayer&&!die)
        {
            if (!isLocalPlayer) return;
            Player_Move();
            if (!isAttack && Input.GetKeyDown(KeyCode.Space))
            {
                Player_Attack();
            }
        }
    }

    private void Player_Move()
    {
        Velocity = WalkSpeed;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(h, 0, v).normalized;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isrun = true;
            Velocity = RunSpeed;
            anim.SetBool("isRun", isrun);
        }
        else
        {
            isrun = false;
            anim.SetBool("isRun", isrun);
            Velocity = WalkSpeed;

        }

        if (Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0)
        {
            iswalk = true;
            anim.SetBool("isWalk", iswalk);
        }
        else
        {
            iswalk = false;
            anim.SetBool("isWalk", iswalk);
        }
        transform.position += moveDirection * Velocity * Time.deltaTime;


        //플레이어가 이동 방향을 바라보도록 회전
        if (moveDirection != Vector3.zero)
        {
            //transform.rotation = Quaternion.Slerp(
            //    transform.rotation,
            //    Quaternion.LookRotation(moveDirection),
            //    RotationSpeed * Time.deltaTime
            //);

            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    [Command]
    void Player_Attack()
    {        
        RpcAttack();        
    }


    [ClientRpc]
    void RpcAttack()
    {
        StartCoroutine(Player_AttackCoroutine());
    }
    [ClientRpc]
    void RpcKillLog()
    {
        killLogUi?.DisplayKillLog(playerNumber);
    }


    IEnumerator Player_AttackCoroutine()
    {
        isAttack = true;
        anim.SetTrigger("Attack");
        AttacktCount++;
        yield return new WaitForSeconds(Attack_Cool);
        isAttack = false;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack")&&health!=0)
        {
            --health;
            if (health==0)
            {
                anim.SetTrigger("Die");
                die = true;
                RpcKillLog();
            }
        }
    }

    public void OnAttackColider()
    {
        attack_col.enabled = true;
    }

    public void OffAttackColider()
    {
        attack_col.enabled = false;
    }



    


    void PlayerNumberChanged(byte _, byte newPlayerNumber)
    {
        OnPlayerNumberChanged?.Invoke(newPlayerNumber);
    }

    void PlayerColorChanged(Color32 _, Color32 newPlayerColor)
    {
        OnPlayerColorChanged?.Invoke(newPlayerColor);
    }

    void PlayerDataChanged(ushort _, ushort newPlayerData)
    {
        OnPlayerDataChanged?.Invoke(newPlayerData);
    }

    #endregion

    #region Server
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        playersList.Add(this);

        

        playerColor = Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);

        //플레이어 닉네임
        playerName = (string)connectionToClient.authenticationData;

        //플레이어 공격으로 변경
        //playerData = (ushort)Random.Range(100, 1000);
        playerData = 0;
        InvokeRepeating(nameof(UpdateData), 0.5f, 1);
    }

    

    //여기서 플레이어의 이름을 받아올것
    [ServerCallback]
    internal static void ResetPlayerNumbers()
    {
        byte playerNumber = 0;
        foreach (TestPlayerMove player in playersList)
            player.playerNumber = playerNumber++;
    }

    
    [ServerCallback]
    void UpdateData()
    {
        playerData = (ushort)AttacktCount;
    }

    
    public override void OnStopServer()
    {
        CancelInvoke();
        playersList.Remove(this);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        playerUIObject = Instantiate(playerUIPrefab, CanvasTestUI.GetPlayersPanel());
        playerUI = playerUIObject.GetComponent<PlayerTestUI>();       


        OnPlayerNumberChanged = playerUI.OnPlayerNumberChanged;
        OnPlayerColorChanged = playerUI.OnPlayerColorChanged;
        OnPlayerDataChanged = playerUI.OnPlayerDataChanged;

        OnPlayerNumberChanged.Invoke(playerNumber);
        OnPlayerColorChanged.Invoke(playerColor);
        OnPlayerDataChanged.Invoke(playerData);
    }
    
    
    public override void OnStartLocalPlayer()
    {
        playerUI.SetLocalPlayer();


        KillLogUIObject = Instantiate(killLogUIPrefab, CanvasTestUI.GetMainPlayersPanel());
        killLogUi = KillLogUIObject.GetComponent<KillLogUi>();


        CanvasTestUI.SetActive(true);
    }

    
    public override void OnStopLocalPlayer()
    {
        CanvasTestUI.SetActive(false);
    }

    
    public override void OnStopClient()
    {
        OnPlayerNumberChanged = null;
        OnPlayerColorChanged = null;
        OnPlayerDataChanged = null;

        Destroy(playerUIObject);
        Destroy(KillLogUIObject);
    }

    #endregion
}

