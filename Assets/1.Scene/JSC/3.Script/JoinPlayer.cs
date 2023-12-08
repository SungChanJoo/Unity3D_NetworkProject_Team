using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinPlayer : MonoBehaviour
{
    [SerializeField] private RPCControll rpcControll;
    private void Awake()
    {
        rpcControll = FindObjectOfType<RPCControll>();
    }
}
