using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class JoinPlayer : NetworkBehaviour
{
    PlayerInfo playerinfo;
    [SerializeField] private RPCControll rpcControll;
    private void Awake()
    {
        rpcControll = FindObjectOfType<RPCControll>();
    }
    public override void OnStartClient()
    {
        

        if (rpcControll.PlayerList.Count == 0)
        {
            playerinfo = new PlayerInfo(SQLManager.Instance.Info.User_name, true);

        }
        else
        {
            playerinfo = new PlayerInfo(SQLManager.Instance.Info.User_name, false);

        }
        rpcControll.PlayerList.Add(this);
        for (int i = 0; i < rpcControll.PlayerList.Count; i++)
        {
            Debug.Log(rpcControll.PlayerList[i].playerinfo.name + " | " + rpcControll.PlayerList[i].playerinfo.isFirst);
        }
    }
}
