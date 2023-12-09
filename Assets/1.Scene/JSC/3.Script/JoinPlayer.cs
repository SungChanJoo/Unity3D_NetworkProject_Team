using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class JoinPlayer : NetworkBehaviour
{
    [SyncVar] public string playerName;
    [SyncVar] public bool isFirstPlayer;
    [SerializeField] private RPCControll rpcControll;
    private void Awake()
    {
        rpcControll = FindObjectOfType<RPCControll>();
    }
    public override void OnStartClient()
    {
        CmdAddPlayer(SQLManager.Instance.Info.User_name, rpcControll.PlayerList.Count == 0);
    }

    [Command]
    public void CmdAddPlayer(string userName, bool isFirst)
    {
        RpcAddPlayer(userName, isFirst);
    }

    [ClientRpc]
    public void RpcAddPlayer(string userName, bool isFirst)
    {
        playerName = userName;
        isFirstPlayer = isFirst;

        rpcControll.PlayerList.Add(new PlayerInfo(userName, isFirst));

        for (int i = 0; i < rpcControll.PlayerList.Count; i++)
        {
            Debug.Log(rpcControll.PlayerList[i].name + " | " + rpcControll.PlayerList[i].isFirst);
        }
    }
}
