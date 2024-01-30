using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomSettingUI : SettingUI
{
    public void ExitGame()
    {
        var manager = RoomManager.singleton;

        if (manager.mode == Mirror.NetworkManagerMode.Host)
        {
            manager.StopHost();
        }
        else if (manager.mode == Mirror.NetworkManagerMode.ClientOnly)
        {
            manager.StopClient();
        }
    }
}
