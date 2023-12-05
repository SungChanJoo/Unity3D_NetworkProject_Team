using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbAI : MonoBehaviour
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

}
