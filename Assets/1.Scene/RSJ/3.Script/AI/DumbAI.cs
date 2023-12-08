using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DumbAI : NetworkBehaviour
{
    public enum State
    {
        idle =0,
        walk,
        playing
    }

    public float speed = 5f;
    private float idle_Time; // ������ ���� �ð�
    private float walk_Time; // ������ �ð�
    private float accum_Time = 0f; // �����ϴ� �ð�
    private float rotation_y;

    private State initState = State.idle;
    private State currentState;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentState = initState;
        idle_Time = Random.Range(0f, 3f);

    }

    [ServerCallback]
    private void Update()
    {
        if (currentState == State.idle) // ������ �ִ� ���¸�
        {
            Idle();
        }
        else if (currentState == State.walk)
        {
            Walk();
        }
    }

    private void Idle()
    {
        accum_Time += Time.deltaTime;

        if (accum_Time < idle_Time) // �������� �־��� �ð����� Idle���¸� �����Ѵ�.
        {
            animator.SetBool("Idle", true);
        }
        else // �� �ð��� ������ �����ð� �ٽ� 0����, walk�� �� �غ�
        {
            accum_Time = 0;
            SetWalk();
        }
    }

    private void Walk()
    {
        accum_Time += Time.deltaTime;

        if (accum_Time < idle_Time) // Walk ���µ���
        {
            animator.SetBool("Walk", true);
            WalkMove();
        }
        else // walk ���� ������ Idle �� �غ�
        {
            accum_Time = 0;
            SetIdle();
        }
    }

    // Walk -> Idle
    private void SetIdle()
    {
        animator.SetBool("Walk", false);
        currentState = State.idle;
        idle_Time = Random.Range(0f, 3f);
    }

    // Idle -> Walk
    private void SetWalk()
    {
        animator.SetBool("Idle", false);
        currentState = State.walk;
        walk_Time = Random.Range(0f, 3f);
        rotation_y = Random.Range(0f, 360f);
        this.transform.rotation = Quaternion.Euler(0, rotation_y, 0);
    }

    private void WalkMove()
    {
        this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    [ClientRpc] // �������� ����ǰ� Ŭ���̾�Ʈ�鿡�� ���� ����
    private void RpcPlayDieAnimation()
    {
        animator.SetTrigger("Die");
        this.gameObject.GetComponent<DumbAI>().enabled = false;
        Destroy(gameObject, 2f);
    }

    [Command(requiresAuthority = false)] // Ŭ���̾�Ʈ -> ���� ȣ�� , �������� ������ ó���ش޶�� ��û��, �׷��� �� �޼��� �ȿ� �ִ� �޼��尡 �������� ����
    private void CmdHandleAttack()
    {
        RpcPlayDieAnimation();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Attack"))
        {
            CmdHandleAttack();
        }
    }

    //[Command]
    //private void RpcAuthorityToClient()
    //{
    //    // Ŭ���̾�Ʈ���� ������ �ο��� ������Ʈ
    //    GameObject objectAuthority = this.gameObject;

    //    // NetworkIdentity ���� ��������
    //    NetworkIdentity networkIdentity = this.gameObject.GetComponent<NetworkIdentity>();

    //    if (networkIdentity != null)
    //    {
    //        //��Ʈ��ũ ������Ʈ�� ���� ������ Ư�� Ŭ���̾�Ʈ���� �ο�
    //        networkIdentity.AssignClientAuthority(connectionToClient);
    //        Debug.Log(connectionToClient);
    //    }
    //}

    //[Command]
    //private void CmdHandleAuthorityToClient()
    //{
    //    RpcAuthorityToClient();
    //}

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();

    //    // Client���� Object�� ���� ���Ѻο�
    //    CmdHandleAuthorityToClient();
    //}

    


}
