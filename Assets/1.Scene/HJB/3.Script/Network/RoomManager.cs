using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{

    //서버에서 새로 접속한 클라이언트를 감지했을 때 동작하는 함수
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        var player = Instantiate(spawnPrefabs[0]);
        //NetworkServer.Spawn 함수로 클라이언트들에게 소환되었음을 전달
        NetworkServer.Spawn(player, conn);

        //conn 매개변수에 방금 서버에 접소한 플레이어의 정보를 담고 있는 networkConnection을
        //전달함으로써 방금 소환된 오브젝트가 새로접속한 플레이어의 소유임을 확인
    }
}
