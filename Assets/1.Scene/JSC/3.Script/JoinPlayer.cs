using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class JoinPlayer : NetworkBehaviour
{
    [SyncVar]
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
            // ���� �÷��̾��� ���, �̸� ����
            CmdSetPlayer(SQLManager.Instance.Info.User_name, GameManager.Instance.PlayerList.Count == 0);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayer(string newName, bool isFirst)
    {
        playerName = newName;
        isFirstPlayer = isFirst;
        EnterPlayer();
    }
    public void EnterPlayer()
    {
        GameManager.Instance.AddPlayerOnServer(this);
    }

}
