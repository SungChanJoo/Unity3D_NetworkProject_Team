using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using LitJson;

/*
 SQL ���� �ܰ�
 0. Ŀ�ؼ� ����
 1. Ŀ�ؼ� ����
 2.������ ������ ����
 { 
    1. connection open ��Ȳ���� Ȯ��
    2. reader ���°� �а� �ִ� ��Ȳ���� Ȯ��
        -�� �������� �Ѱ���
    3.�����͸� �� �о����� Reader�� ���� Ȯ�� �� Close
 }
 */
public class user_info
{
    public string User_name { get; private set; }
    public string User_Password { get; private set; }
    public string User_Nickname { get; private set; }

    public user_info(string name, string password)
    {
        User_name = name;
        User_Password = password;
    }
    public user_info(string name, string password, string nickname)
    {
        User_name = name;
        User_Password = password;
        User_Nickname = nickname;
    }
}

public class SQLManager : MonoBehaviour
{
    public user_info Info;

    public MySqlConnection Connection;
    public MySqlDataReader Reader;
    

    public string DB_Path = string.Empty;

    public static SQLManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DB_Path = Application.dataPath + "/Database";
        string serverInfo = ServerSet(DB_Path);
        try
        {
            if (serverInfo.Equals(string.Empty))
            {
                Debug.Log("SQL Server Json Error!");
                return;
            }
            Connection = new MySqlConnection(serverInfo);
            Connection.Open();
            Debug.Log("SQL Server Complete!");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private string ServerSet(string path)
    {
        if (!File.Exists(path))// �� ��ο� ������ �ֳ���?
        {
            Directory.CreateDirectory(path);
        }

        string jsonString = File.ReadAllText(path + "/config.json");

        JsonData itemData = JsonMapper.ToObject(jsonString);
        string serverInfo = $"Server = {itemData[0]["IP"]};" +
                            $"Database = {itemData[0]["TableName"]};" +
                            $"Uid = {itemData[0]["ID"]};" +
                            $"Pwd = {itemData[0]["PW"]};" +
                            $"Port = {itemData[0]["PORT"]};" +
                            $"CharSet=utf8;";
        Debug.Log(serverInfo);
        return serverInfo;
    }

    private bool ConnectionCheck(MySqlConnection con)
    {
        //���� MySqlConnection open ���°� �ƴ϶��?
        if (con.State != System.Data.ConnectionState.Open)
        {
            con.Open();
            if (con.State != System.Data.ConnectionState.Open)
            {
                return false;
            }
        }
        return true;
    }

    public bool Join(string id, string password, string nickname)
    {
        string SQLCommand;
        try
        {
            if(!ConnectionCheck(Connection))
            {
                Debug.Log("�������");
                return false;
            }
            SQLCommand = string.Format(@"SELECT User_Name FROM user_info WHERE User_Name = '{0}';", id);
            MySqlCommand cmd = new MySqlCommand(SQLCommand, Connection);
            Reader = cmd.ExecuteReader();
            //Reader ���� �����Ͱ� 1�� �̻� ������?
            if (Reader.HasRows)
            {
                //���� �����͸� �ϳ��� �����մϴ�.
                while (Reader.Read())
                {
                    /*
                     ���׿�����
                    */
                    string name = (Reader.IsDBNull(0)) ? string.Empty : (string)Reader["User_Name"].ToString();
                    if (!name.Equals(string.Empty))
                    {
                        //Id �� �̹� �����ϴ� ��Ȳ
                        //ȸ������ ����
                        break;
                    }
                }//end while
            }//end if
            else
            {
                if (!Reader.IsClosed) Reader.Close();

                //ȸ������ ����
                SQLCommand = string.Format(@"INSERT INTO user_info(User_Name, User_Password, User_Nickname) VALUES('{0}','{1}','{2}');", id, password, nickname);
                MySqlCommand joinCmd = new MySqlCommand(SQLCommand, Connection);
                if (joinCmd.ExecuteNonQuery() == 1)
                {
                    //���������� data�� �ҷ��� ��Ȳ
                    Info = new user_info(id, password, nickname);
                }
                else
                {
                    Debug.Log("�μ�Ʈ ����");
                }
                return true;
            }

            if (!Reader.IsClosed) Reader.Close();
            return false;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            if (!Reader.IsClosed) Reader.Close();
            return false;

        }
    }

    public bool Login(string id, string password)
    {
        //���������� DB���� �����͸� ������ ���� �޼ҵ�
        //��ȸ�Ǵ� �����Ͱ� ���ٸ� false
        //��ȸ�� �Ǵ� �����Ͱ� �ִٸ� true���� �����µ�
        //������ ������ info���ٰ� ���� ������ ����

        /*
            1. connection open ��Ȳ���� Ȯ��
            2. reader ���°� �а� �ִ� ��Ȳ���� Ȯ��
                -�� �������� �Ѱ���
            3.�����͸� �� �о����� Reader�� ���� Ȯ�� �� Close
         */

        try
        {
            if (!ConnectionCheck(Connection))
            {
                Debug.Log("�������");
                return false;
            }

            string SQLCommand = string.Format(@"SELECT User_Name, User_Password FROM user_info
                                                WHERE User_Name='{0}' AND User_Password = '{1}';", id, password);

            MySqlCommand cmd = new MySqlCommand(SQLCommand, Connection);
            Reader = cmd.ExecuteReader();
            //Reader ���� �����Ͱ� 1�� �̻� ������?
            if (Reader.HasRows)
            {
                //���� �����͸� �ϳ��� �����մϴ�.
                while (Reader.Read())
                {
                    /*
                     ���׿�����
                    */
                    string name = (Reader.IsDBNull(0)) ? string.Empty : (string)Reader["User_Name"].ToString();
                    string pass = (Reader.IsDBNull(1)) ? string.Empty : (string)Reader["User_Password"].ToString();
                    string nick = (Reader.IsDBNull(2)) ? string.Empty : (string)Reader["User_Nickname"].ToString();

                    if (!name.Equals(string.Empty) || !pass.Equals(string.Empty) || !nick.Equals(string.Empty))
                    {
                        //���������� data�� �ҷ��� ��Ȳ
                        Info = new user_info(name, pass, nick);

                        //Reader�� �����ִ�?
                        if (!Reader.IsClosed) Reader.Close();
                        return true;
                    }
                    else
                    {
                        //�α��� ����
                        break;
                    }
                }//end while

            }//end if

            if (!Reader.IsClosed) Reader.Close();
            return false;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            if (!Reader.IsClosed) Reader.Close();
            return false;
        }

    }
}
