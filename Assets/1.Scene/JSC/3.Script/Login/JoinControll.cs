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
                _log.text += "\n���̵� �Է��ϼ���";
            }
            if (Pass_i.text.Equals(string.Empty))
            {
                _log.text += "\n��й�ȣ�� �Է��ϼ���";
            }
            if (Nick_i.text.Equals(string.Empty))
            {
                _log.text += "\n�г����� �Է��ϼ���";
            }
            return;
        }
        if(SQLManager.Instance.Join(Id_i.text, Pass_i.text, Nick_i.text))
        {
            //ȸ������ ����
            user_info info = SQLManager.Instance.Info;
            Debug.Log(info.User_name + " | " + info.User_Password + " | " + info.User_Nickname);
            gameObject.SetActive(false);
        }
        else
        {
            //ȸ������ ����
            _log.text = "\n�ߺ��� ���̵� �Դϴ�.";
        }
    }

    public void CloseBtn()
    {
        JoinUI.SetActive(false);
    }
}
