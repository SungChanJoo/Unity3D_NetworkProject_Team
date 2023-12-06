using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class EnterRoom : MonoBehaviour
{
    [SerializeField] private InputField PlayerNameInputField;
    [SerializeField] private InputField roomNameInputField;

    public void onClickCreateRoom()
    {
        if (roomNameInputField.text != "")
        {
            SettingUI.RoomName = roomNameInputField.text;
            //����� ����
            var manager = RoomManager.singleton;
            //�漳�� �۾�
            manager.StartHost();
        }
        else
        {
            Debug.Log("���̸��� �Է����ּ���");
            //�ؽ�Ʈ�� ����ִٸ�
            return;
        }
    }

    public void onClickEnterGameRoomButton()
    {
        if (PlayerNameInputField.text != "")
        {
            var manager = RoomManager.singleton;
            manager.StartClient();
        }
        else
        {
            Debug.Log("�̸��� �Է����ּ���");
        }
    }
}
