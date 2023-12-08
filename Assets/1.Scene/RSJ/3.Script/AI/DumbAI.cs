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
    private float idle_Time; // 가만히 있을 시간
    private float walk_Time; // 움직일 시간
    private float accum_Time = 0f; // 축적하는 시간
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
        if (currentState == State.idle) // 가만히 있는 상태면
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

        if (accum_Time < idle_Time) // 랜덤으로 주어진 시간동안 Idle상태를 유지한다.
        {
            animator.SetBool("Idle", true);
        }
        else // 그 시간이 지나면 축적시간 다시 0으로, walk로 갈 준비
        {
            accum_Time = 0;
            SetWalk();
        }
    }

    private void Walk()
    {
        accum_Time += Time.deltaTime;

        if (accum_Time < idle_Time) // Walk 상태동안
        {
            animator.SetBool("Walk", true);
            WalkMove();
        }
        else // walk 상태 끝나면 Idle 갈 준비
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

    [ClientRpc] // 서버에서 실행되고 클라이언트들에게 정보 전달
    private void RpcPlayDieAnimation()
    {
        animator.SetTrigger("Die");
        this.gameObject.GetComponent<DumbAI>().enabled = false;
        Destroy(gameObject, 2f);
    }

    [Command(requiresAuthority = false)] // 클라이언트 -> 서버 호출 , 서버에게 정보를 처리해달라고 요청함, 그러면 저 메서드 안에 있는 메서드가 서버에서 실행
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
    //    // 클라이언트에게 권한을 부여할 오브젝트
    //    GameObject objectAuthority = this.gameObject;

    //    // NetworkIdentity 변수 가져오기
    //    NetworkIdentity networkIdentity = this.gameObject.GetComponent<NetworkIdentity>();

    //    if (networkIdentity != null)
    //    {
    //        //네트워크 오브젝트에 대한 권한을 특정 클라이언트에게 부여
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

    //    // Client에게 Object에 대한 권한부여
    //    CmdHandleAuthorityToClient();
    //}

    


}
