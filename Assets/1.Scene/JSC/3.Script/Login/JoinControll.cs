using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JoinControll : MonoBehaviour
{
    public InputField Id_i;
    public InputField Pass_i;
    public InputField Nick_i;

    [SerializeField] private Text _log;
    [SerializeField] private GameObject JoinUI;

    public void JoinBtn()
    {
        _log.text = "";
        if (Id_i.text.Equals(string.Empty) || Pass_i.text.Equals(string.Empty) || Nick_i.text.Equals(string.Empty))
        {
            if (Id_i.text.Equals(string.Empty))
            {
                _log.text += "\n아이디를 입력하세요";
            }
            if (Pass_i.text.Equals(string.Empty))
            {
                _log.text += "\n비밀번호를 입력하세요";
            }
            if (Nick_i.text.Equals(string.Empty))
            {
                _log.text += "\n닉네임을 입력하세요";
            }
            return;
        }
        if(SQLManager.Instance.Join(Id_i.text, Pass_i.text, Nick_i.text))
        {
            //회원가입 성공
            user_info info = SQLManager.Instance.Info;
            Debug.Log(info.User_name + " | " + info.User_Password + " | " + info.User_Nickname);
            gameObject.SetActive(false);
        }
        else
        {
            //회원가입 실패
            _log.text = "\n중복된 아이디 입니다.";
        }
    }

    public void CloseBtn()
    {
        JoinUI.SetActive(false);
    }
}
