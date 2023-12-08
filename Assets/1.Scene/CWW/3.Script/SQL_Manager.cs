using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using LitJson;

public class user_info_
{
    public string User_Name { get; private set; }
    public string User_Password { get; private set; }
    public user_info_(string name, string password)
    {
        User_Name = name;
        User_Password = password;
    }
}

public class SQL_Manager : MonoBehaviour
{
    /*
     sql ��������
     connection ���� -> ����
     ������ ��ȸ(��������)
    1. ���»��� Ȯ��
    2. ���� ���°� �а� �ִ� ��Ȳ���� Ȯ�� -�������� �Ѱ�
    3. �������� Ȯ���� close
     */
    public user_info info;

    public MySqlConnection connection;
    public MySqlDataReader reader;

    public string DB_Path = string.Empty;

    public static SQL_Manager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DB_Path = Application.dataPath + "/Database";
        string serverinfo = ServerSet(DB_Path);

        try
        {
            if (serverinfo.Equals(string.Empty))
            {
                print("SQL Server Json Err!");
                return;
            }
            connection = new MySqlConnection(serverinfo);
            connection.Open();
            print("Sql Server Open Complete!");
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

    private string ServerSet(string path)
    {
        if (!File.Exists(path)) //�� ��ο� ������ ���ٸ�
        {
            Directory.CreateDirectory(path);
        }
        string JsonString = File.ReadAllText(path + "/config.json");
        JsonData itemdata = JsonMapper.ToObject(JsonString);
        string serverInfo = $"Server={itemdata[0]["IP"]};" +
             $"Database={itemdata[0]["TableName"]};" +
             $"Uid={itemdata[0]["ID"]};" +
             $"Pwd={itemdata[0]["PW"]};" +
             $"Port={itemdata[0]["PORT"]};" +
             $"charset=utf8;";
        return serverInfo;
    }

    private bool connection_check(MySqlConnection con)
    {
        //���� MySqlConnection open�� �ƴ϶��?
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
    public bool Login(string id, string password)
    {
        //����������DB���� �����͸� ������ ���� �޼ҵ�
        //��ȸ�Ǵ� �����Ͱ� ���ٸ� False
        //��ȸ�� �Ǵ� �����Ͱ� �ִٸ� true ���� ������ �� 
        //������ ������ info���ٰ� ���� ������ ��������

        try
        {
            //1.connction open Ȯ��
            if (!connection_check(connection))
            {
                return false;
            }
            string SQL_command = string.Format(@"SELECT USER_NAME,USER_PASSWORD FROM USER_INFO
                    WHERE USER_NAME = '{0}' AND User_password = '{1}';", id, password);
            MySqlCommand cmd = new MySqlCommand(SQL_command, connection);
            reader = cmd.ExecuteReader();
            //Reader ���� �����Ͱ� 1���̻� �����ϴ�?
            if (reader.HasRows)
            {
                //���� �����͸� �ϳ��� ����
                while (reader.Read())
                {
                    //���׿�����
                    string name = (reader.IsDBNull(0)) ? string.Empty : (string)reader["USER_NAME"].ToString();
                    string pass = (reader.IsDBNull(1)) ? string.Empty : (string)reader["USER_PASSWORD"].ToString();
                    if (!name.Equals(string.Empty) || !name.Equals(string.Empty))
                    {
                        //���������� Data�� �ҷ��� ��Ȳ
                        info = new user_info(name, pass);
                        if (!reader.IsClosed) reader.Close();
                        return true;
                    }
                    else // �α��ν���
                    {
                        break;
                    }
                }
            }
            if (!reader.IsClosed) reader.Close();
            return false;
        }
        catch (Exception e)
        {
            print(e.Message);
            if (!reader.IsClosed) reader.Close();
            return false;
        }
    }

}
