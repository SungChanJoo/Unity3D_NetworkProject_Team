using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetUI_Manager : NetworkManager
{
    public static new NetUI_Manager singleton { get; private set; }

    
    public override void Awake()
    {
        base.Awake();
        singleton = this;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        TestPlayerMove.ResetPlayerNumbers();
    }
   
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        TestPlayerMove.ResetPlayerNumbers();
    }
}

