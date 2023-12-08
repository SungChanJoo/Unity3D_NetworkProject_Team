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

    [Header("Camera")]
    [SerializeField] private Camera camera;

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
    private void Awake()
    {
        TryGetComponent(out anim);
        camera = GameObject.Find("Camera").GetComponent<Camera>();
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
        CoolTime();
        if (this.isLocalPlayer) //자기자신인지 확인하는 용도 network에서 .
        {
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
                StartCoroutine(CreateRunEffect());
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


    [Command]
    private void Start_Player_Attack()
    {
        StartCoroutine(Player_Attack());
    }
    private IEnumerator Player_Attack()
    {
        isAttack = true;
        isAttackCool = true;
        anim.SetTrigger("Attack");
        AnimationClip Attack = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Attack");
        yield return new WaitForSeconds(Attack.length);
        isAttack = false;
        yield return new WaitForSeconds(Attack_Cool - Attack.length);
        isAttackCool = false;

    }

    [ClientRpc]
    public void OnAttackColider()
    {
        if (this.isLocalPlayer) attack_col.enabled = true;
    }
    [ClientRpc]
    public void OffAttackColider()
    {
        if(this.isLocalPlayer) attack_col.enabled = false;
    }





}
