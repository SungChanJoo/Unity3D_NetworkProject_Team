using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class JoinPlayer : NetworkBehaviour
{
/*    [SyncVar(hook = nameof(EnterPlayer))]
    public string playerName = "난 바보야 ㅠㅠ";
    [SyncVar(hook = nameof(OnIsFirstPlayerChanged))]*/
    public bool isFirstPlayer;
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    private void OnPlayerNameChanged(string oldName, string newName)
    {
        Debug.Log($"Player name changed: {oldName} -> {newName}");
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            // 로컬 플레이어인 경우, 이름 설정
            CmdSetPlayerName(SQLManager.Instance.Info.User_name);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayerName(string newName)
    {
        playerName = newName;
    }
    public void EnterPlayer(string oldName, string newName)
    {
        Debug.Log("일단 EnterPlaye : " + oldName + " -> " + newName);
        GameManager.Instance.AddPlayerOnServer(this);
    }
    /*
        private void OnIsFirstPlayerChanged(bool oldValue, bool newValue)
        {
            // isFirstPlayer가 변경될 때 호출되는 콜백
            Debug.Log("isFirstPlayer changed: " + newValue);
        }

        public override void OnStartClient()
        {
            if (!isLocalPlayer) return;
            playerName = SQLManager.Instance.Info.User_name;
            isFirstPlayer = GameManager.PlayerList.Count == 0;
            CmdSetPlayerInfo(playerName, isFirstPlayer);
        }

        [Command(requiresAuthority = false)]
        private void CmdSetPlayerInfo(string userName, bool isFirst)
        {
            RpcSetPlayerInfo(userName, isFirst);
        }

        [ClientRpc]
        private void RpcSetPlayerInfo(string userName, bool isFirst)
        {
            playerName = userName;
            isFirstPlayer = isFirst;
            Debug.Log("Player info set: " + playerName + " | " + isFirstPlayer);
        }*/

    /*    public override void OnStartLocalPlayer()
        {
            CmdAddPlayer(SQLManager.Instance.Info.User_name, GameManager.PlayerList.Count == 0);
        }

        [Command(requiresAuthority = false)]
        public void CmdAddPlayer(string userName, bool isFirst)
        {
            RpcAddPlayer(userName, isFirst);
            GameManager.Instance.AddPlayerOnServer(this);

        }
        [ClientRpc]
        public void RpcAddPlayer(string userName, bool isFirst)
        {
            playerName = userName;
            isFirstPlayer = isFirst;
        }*/


    /*    [ClientRpc]
        public void RpcAddPlayer(string userName, bool isFirst)
        {
            playerName = userName;
            isFirstPlayer = isFirst;
            Debug.Log(playerName + " | " + isFirstPlayer);
            for (int i = 0; i < RPCControll.Instance.PlayerList.Count; i++)
            {
                Debug.Log(RPCControll.Instance.PlayerList[i].name + " | " + RPCControll.Instance.PlayerList[i].isFirst);
            }
        }*/
}
