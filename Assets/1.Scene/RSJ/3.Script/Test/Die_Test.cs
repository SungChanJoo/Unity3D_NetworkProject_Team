using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Die_Test : NetworkBehaviour
{
    private Animator anim;
    private BoxCollider colider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [ClientRpc]
    private void RpcDead()
    {
        if (anim != null)
        {
            anim.SetTrigger("Die");
            //this.gameObject.GetComponent<PlayerMove_Test_NotServer>().enabled = false;
            Destroy(gameObject, 2f);
        }
    }
    

    [Command(requiresAuthority = false)]
    private void CmdHandleAttack()
    {
        RpcDead();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            CmdHandleAttack();
        }
    }
}
