using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
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
    private void Awake()
    {
        TryGetComponent(out anim);
    }

    void Update()
    {
        Player_Move();
        if (!isAttack && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Player_Attack());
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

    private IEnumerator Player_Attack()
    {
        isAttack = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(Attack_Cool);
        isAttack = false;
    }


    public void OnAttackColider()
    {
        attack_col.enabled = true;
    }

    public void OffAttackColider()
    {
        attack_col.enabled = false;
    }


}
