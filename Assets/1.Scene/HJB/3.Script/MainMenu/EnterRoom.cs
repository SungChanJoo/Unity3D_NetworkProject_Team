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
            //방생성 로직
            var manager = RoomManager.singleton;
            //방설정 작업
            manager.StartHost();
        }
        else
        {
            Debug.Log("방이름을 입력해주세요");
            //텍스트가 비어있다면
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
            Debug.Log("이름을 입력해주세요");
        }
    }
}
