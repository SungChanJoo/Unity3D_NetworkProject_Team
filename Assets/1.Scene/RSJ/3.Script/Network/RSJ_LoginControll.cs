using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginControll : MonoBehaviour
{
    public InputField id_i;
    public InputField Pass_i;

    [SerializeField] private Text Log;


    public void Login_btn()
    {
        if(id_i.text.Equals(string.Empty) || Pass_i.text.Equals(string.Empty))
        {
            Log.text = "���̵� ��й�ȣ�� �Է��ϼ���.";
            return;
        }

        if(SQL_Manager.instance.Login(id_i.text,Pass_i.text))
        {
            //�α��� ����

            user_info info = SQL_Manager.instance.info;
            Debug.Log(info.User_name + " | " + info.User_Password);
            gameObject.SetActive(false);
        }
        else
        {
            //�α��� ����
            Log.text = "���̵� ��й�ȣ�� Ȯ�����ּ���.";
        }
    }

}
