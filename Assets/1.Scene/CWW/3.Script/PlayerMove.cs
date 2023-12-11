using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using Mirror;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineFreeLook LookatCamera;
    [SerializeField] private float WalkSpeed = 10f;
    [SerializeField] private float RunSpeed = 15f;
    [SerializeField] private float yVelocity = 0;
    [SerializeField] private float xVelocity = 0;
    [SerializeField] private float RotationSpeed = 3f;

    [SerializeField] private GameObject RunParticle_Prefab;

    [Header("Attack_Colider")]
    [SerializeField] private Collider attack_col;

    [SerializeField] private Animator anim;
    [SerializeField] private NetworkAnimator networkAnimator;

    [Header("Camera")]
    [SerializeField] private Camera camera;

    [Header("AttackSound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attactSound;
    [SerializeField] private AudioClip DieSound;

    private float Velocity;
    private float Stop = 0;

    private bool iswalk = false;
    private bool isrun = false;
    private bool isAttack = false;
    private bool isAttackCool = false;

    float cooldownTimer = 0.0f;
    bool isCooldown = false;

    [Header("Att_cool")]
    [SerializeField] private float Attack_Cool = 0f;

    JoinPlayer joinPlayer;
    [SyncVar] private bool isDie = false;
    private void Awake()
    {
        TryGetComponent(out anim);
        networkAnimator = GetComponent<NetworkAnimator>();
        camera = GameObject.Find("Camera").GetComponent<Camera>();
        joinPlayer = GetComponent<JoinPlayer>();
        
     //   Debug.Log("플레이어 : " + joinPlayer.playerName + " | " + joinPlayer.isFirstPlayer);
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();


        Cinemachine.CinemachineFreeLook freeLookCamera = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        if (!isLocalPlayer) return;
        if (freeLookCamera != null)
        {
            // 현재 로컬 플레이어에 따라가도록 설정
            freeLookCamera.Follow = transform;
            freeLookCamera.LookAt = transform;
        }
    }
    void Update()
    {
        
        if (this.isLocalPlayer) //자기자신인지 확인하는 용도 network에서 .
        {
            CoolTime();
            if (!isAttack)
            {
                Player_Move();
            }

            if (!isAttack && Input.GetKeyDown(KeyCode.Space) && !isAttackCool)
            {
                Start_Player_Attack();
            }
        }


    }


    private void Player_Move()
    {
        Velocity = WalkSpeed;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        Vector3 moveDirection = (v * cameraForward + h * cameraRight).normalized;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isrun = true;
            Velocity = RunSpeed;
            anim.SetBool("isRun", isrun);
            if (!isCooldown)
            {
                CmdCreateRunEffect();
            }
        }
        else
        {
            isrun = false;
            anim.SetBool("isRun", isrun);
            Velocity = WalkSpeed;
        }

        if(Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0)
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
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDirection),
                RotationSpeed * Time.deltaTime
            );
        }
    }

    [Command]
    private void CmdCreateRunEffect()
    {
        RPCCreateUnEffect();
    }
    [ClientRpc]
    private void RPCCreateUnEffect()
    {
        StartCoroutine(CreateRunEffect());
    }
    IEnumerator CreateRunEffect()
    {
        Vector3 offset = new Vector3(0, 1.5f, 0);
        GameObject runEffect = Instantiate(RunParticle_Prefab, transform.position, Quaternion.identity);
        Vector3 startPos = runEffect.transform.position;
        Vector3 endPos = runEffect.transform.position + offset;

        float duration = 1.5f; // 이펙트 이동에 걸리는 시간
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            runEffect.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // 이펙트 파괴
        Destroy(runEffect);
    }

    private void CoolTime()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= 0.3f)
        {
            cooldownTimer = 0.0f;
            isCooldown = false;
        }
        else
        {
            isCooldown = true;
        }
    }


    [Command]
    private void Start_Player_Attack()
    {
        RpcPlayerAttack();
    }
    
    private IEnumerator Player_Attack()
    {
        isAttack = true;
        isAttackCool = true;
        anim.SetTrigger("Attack");
        audioSource.PlayOneShot(attactSound);
        AnimationClip Attack = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Attack");
        yield return new WaitForSeconds(Attack.length);
        isAttack = false;
        yield return new WaitForSeconds(Attack_Cool - Attack.length);
        isAttackCool = false;
    }

    [ClientRpc]
    void RpcPlayerAttack()
    {
        StartCoroutine(Player_Attack());
    }

    public void OnAttackColider()
    {
        if (this.isLocalPlayer) attack_col.enabled = true;
    }
    public void OffAttackColider()
    {
        if(this.isLocalPlayer) attack_col.enabled = false;
    }

    private BoxCollider colider;
    [SerializeField] GameObject[] gameObjects;

    

    [Command(requiresAuthority = false)]
    private void CmdKill(string attacker, string targetPlayer)
    {
        RPCKill(attacker, targetPlayer);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            if (other.transform.root.TryGetComponent(out JoinPlayer player))
            {
                string attackPlayer = player.playerName;
                string targetPlayer = joinPlayer.playerName;
                Debug.Log("재백에 내 문제 아니다" + attackPlayer + " | " + targetPlayer);
                joinPlayer.CmdPlayerDie();
                CmdKill(attackPlayer, targetPlayer);
            }
        }
    }
    [ClientRpc]
    private void RPCKill(string attacker, string targetPlayer)
    {
        anim.SetTrigger("Die");
        isDie = true;

        KillLogUi.instance.DisplayKillLog(attacker, targetPlayer);
        gameObject.SetActive(false);
        //Destroy(gameObject, 2f);
    }


    //클라이언트에서 소리를 서버로
    public void SendSoundToServer()
    {
        CmdSendSoundToServer();
    }

    //서버에서 모든 클라이언트로 소리를 전달
    [ClientRpc]
    void RpcReceiveSoundOnClients()
    {
      //  audioSource.PlayOneShot(audioSource.clip);
    }

    //클라이언트에서 소리를 서버로
    [Command]
    void CmdSendSoundToServer()
    {
        RpcReceiveSoundOnClients();
    }

}
