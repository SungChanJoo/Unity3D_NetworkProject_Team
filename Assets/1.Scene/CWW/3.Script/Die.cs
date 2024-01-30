using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Die : NetworkBehaviour
{
    private Animator anim;
    private BoxCollider colider;
    [SerializeField] GameObject[] gameObjects;

    [SyncVar] private bool isDie = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        colider = GetComponent<BoxCollider>();
    }

    
    [ClientRpc]
    private void RPC_PlayerDie()
    {
        isDie = true;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            RPC_PlayerDie();
            anim.SetTrigger("Die");
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].SetActive(false);
            }
            //gameObject.GetComponent<DumbAI>().enabled = false;
            Destroy(gameObject, 2f);
        }
    }
}
