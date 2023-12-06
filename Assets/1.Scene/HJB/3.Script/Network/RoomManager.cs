using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{

    //�������� ���� ������ Ŭ���̾�Ʈ�� �������� �� �����ϴ� �Լ�
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        var player = Instantiate(spawnPrefabs[0]);
        //NetworkServer.Spawn �Լ��� Ŭ���̾�Ʈ�鿡�� ��ȯ�Ǿ����� ����
        NetworkServer.Spawn(player, conn);

        //conn �Ű������� ��� ������ ������ �÷��̾��� ������ ��� �ִ� networkConnection��
        //���������ν� ��� ��ȯ�� ������Ʈ�� ���������� �÷��̾��� �������� Ȯ��
    }
}
