using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class RPCControll : NetworkBehaviour
{
    [SerializeField] private TMP_Text chatText;
    [SerializeField] private TMP_InputField inputfield;
    [SerializeField] private GameObject canvas;

    private static event Action<string> onMessage;

    //client�� server�� connect�Ǿ����� �ݹ��Լ�
    public override void OnStartAuthority()
    {
        if(isLocalPlayer)
        {
            canvas.SetActive(true);
        }
        onMessage += newMessage;

    }

    private void newMessage(string message)
    {
        chatText.text += message;
    }

    //Ŭ���̾�Ʈ�� Server�� ������ ��
    [ClientCallback]
    private void OnDestroy()
    {
        if(!isLocalPlayer)
        {
            onMessage -= newMessage;
        }
    }

    //RPC�� �ᱹ ClientRpc ��ɾ� < Command(Server)��ɾ� < Client ��ɾ�?
    //Ŭ���̾�Ʈ�� �������� �޼����� ��û�ϸ� ������ ��� Ŭ���̾�Ʈ�鿡�� �� �޼����� ����������.
    //������ �ϳ�?(���) -> Rpc, ����� �����ϳ�?(����) -> �ٸ� ��Ʈ��ũ?, Ŭ���̾�Ʈ�� ��û�ϰ� ������ ����ȭ���� // �� ���̶� Rpc�� �� ����
    //������ �ִ� ��ũ��Ʈ�� Ŭ���̾�Ʈ�� �����ִ� ��ũ��Ʈ�� ���ƾ߸� ������ �ȴ�,. ������ Ŭ���̾�Ʈ�� ��ũ��Ʈ�� �ٸ���� ���� �����ؾ���

    [Client] // ���� Ŭ���̾�Ʈ ����
    private void Send()
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return;
        if (string.IsNullOrWhiteSpace(inputfield.text)) return;
        cmdSendMessage(inputfield.text); //
        inputfield.text = string.Empty;
    }

    [Command] // �������� �ٸ� Ŭ���̾�Ʈ�鿡�� ������ ��� (��������)
    private void cmdSendMessage(string message)
    {
        RpcHandleMessage($"[{connectionToClient.connectionId}] : {message}");
        //Ŭ���̾�ƮRpc�� ������ ������ �޼��带 �� �� ����.
        // transform.position += 1; -> ����
        // Debug(); -> �Ұ���
    }

    /* ���÷� ClientRpc�� ������ ���� Debug��� �޼���
     private void Debug()
    {
        Debug.Log(e.MEsagee);
    }
     */

    [ClientRpc] //�� �޼ҵ带 rpc�� ���̴ϴ�. �׳� �ϳ��� �޼�����. �׳� �޼����ε� rpc�� ����
    private void RpcHandleMessage(string message) // string�Ӹ� �ƴ϶� rigidbody, transform �� � �����̵� �����ϴ�.
    {
        onMessage?.Invoke($"\n{message}");
    }


}
