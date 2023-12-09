using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public string name { get; private set; }
    public bool isFirst { get; private set; }

    public PlayerInfo(string username, bool isFirstPlayer)
    {
        name = username;
        isFirst = isFirstPlayer;
    }
}
