using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginControll : MonoBehaviour
{
    public InputField Id_i;
    public InputField Pass_i;

    [SerializeField] private Text _log;

    public void LoginBtn()
    {
        if (Id_i.text.Equals(string.Empty) || Pass_i.text.Equals(string.Empty))
        {
            _log.text = "아이디 비밀번호를 입력하세요.";
            return;
        }
        if (SQLManager.Instance.Login(Id_i.text, Pass_i.text))
        {
            //로그인 성공
            user_info info = SQLManager.Instance.Info;
            Debug.Log(info.User_name + " | " + info.User_Password + " | " + info.User_Nickname);
            gameObject.SetActive(false);
        }
        else
        {
            //로그인 실패
            _log.text = "아이디 비밀번호를 확인해 주세요..";
        }


    }
}
