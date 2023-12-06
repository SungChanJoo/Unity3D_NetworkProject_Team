using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    private Animator anim;
    private BoxCollider colider;
    [SerializeField] GameObject[] gameObjects;
    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out colider);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            anim.SetTrigger("Die");
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].SetActive(false);
            }
        }
    }
}
