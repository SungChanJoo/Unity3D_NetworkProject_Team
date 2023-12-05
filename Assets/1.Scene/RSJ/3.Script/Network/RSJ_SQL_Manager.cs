using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using LitJson;

public class user_info
{
    public string User_name { get; private set; }
    public string User_Password { get; private set; }

    public user_info(string name, string password)
    {
        User_name = name;
        User_Password = password;
    }
}

public class SQL_Manager : MonoBehaviour
{
    public user_info info;

    public MySqlConnection connection;
    public MySqlDataReader reader;

    public string DB_Path = string.Empty;

    public static SQL_Manager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
            if(serverinfo.Equals(string.Empty))
            {
                Debug.Log("SQL Server Json Error!");
                return;
            }
            connection = new MySqlConnection(serverinfo);
            connection.Open(); // ���� �����Ѱ�
            Debug.Log("SQL Server Open Complete!");
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    private string ServerSet(string path)
    {
        if(!File.Exists(path)) // �� ��ο� ������ �ֳ���?, ��ο� ������ ���ٸ�
        {
            Directory.CreateDirectory(path);
        }
        string Jsonstring = File.ReadAllText(path + "/config.Json");
        /*
         [{"IP" : "192.168.0.75", "TableName '''''''''''''''
         */
        JsonData itemdata = JsonMapper.ToObject(Jsonstring);
        string serverInfo = 
            $"Server={itemdata[0]["IP"]};" +
            $"Database={itemdata[0]["TableName"]};" +
            $"Uid ={itemdata[0]["ID"]};" +
            $"Pwd = {itemdata[0]["PW"]};" +
            $"Port = { itemdata[0]["PORT"]};" +
            $"CharSet = utf8;";

        return serverInfo;
    }

    private bool connection_check(MySqlConnection con)
    {
        //���� MySqlConnectin\on open ���°� �ƴ϶��?
        if(con.State!= System.Data.ConnectionState.Open)
        {
            con.Open();
            if(con.State!=System.Data.ConnectionState.Open) 
            {
                return false; //������� �ߴµ��� �ȵǸ� �̻��� ����
            }
        }
        return true;
    }

    public bool Login(string id, string password)
    {
        //���������� DB���� �����͸� ������ ���� �޼���
        //��ȸ�Ǵ� �����Ͱ� ���ٸ� False
        //��ȸ�� �Ǵ� �����Ͱ� �ִٸ� True���� �����µ�
        //������ ������ Info���ٰ� ���� ������ �����ž�
        /*
         1.Connection open ��Ȳ���� Ȯ�� -> �޼ҵ�ȭ
        2. Reader ���°� �а� �ִ� ��Ȳ���� Ȯ��
         - �� �������� �ϳ�
        3. �����͸� �� �о����� Close
         */

        try
        {
            //1.Connection open ��Ȳ���� Ȯ��->�޼ҵ�ȭ
            if (!connection_check(connection))
            {
                return false;
            }

            string SQL_command =
                string.Format(@"SELECT User_name,User_Password FROM user_info
                                WHERE User_name = '{0}' AND User_Password = '{1}';", id,password); //����̴� �ٹٲ��� �־ ���ٷ� �ν��Ѵٴ� ��, �������� ������ ���ٴ� ���� Ȯ�� �� �����ϱ�
            MySqlCommand cmd = new MySqlCommand(SQL_command, connection);
            reader = cmd.ExecuteReader();
            //Reader ���� �����Ͱ� �Ѱ��̻� ������?
            if(reader.HasRows)
            {
                //���� �����͸� �ϳ��� �����մϴ�.
                while (reader.Read())
                {
                    /*
                     ���׿�����
                     */
                    string name = (reader.IsDBNull(0)) ? string.Empty : (string)reader["User_Name"].ToString();
                    string Pass = (reader.IsDBNull(1)) ? string.Empty : (string)reader["User_Password"].ToString();
                    if(!name.Equals(string.Empty)|| !Pass.Equals(string.Empty))
                    {
                        //���������� �����͸� �ҷ��� ��Ȳ
                        info = new user_info(name, Pass);
                        if (!reader.IsClosed) reader.Close(); // ������ �����ִٸ� �ݾ��ּ���
                        return true;
                    }
                    else // �α��� ����
                    {
                        break;
                    }

                }//while ��
            }// if ��
            if (!reader.IsClosed) reader.Close(); // else������ �������� �ݾ��ֱ�
            return false; //������� ���� �α��ο� �����߱� ������ false ��ȯ

        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            if(!reader.IsClosed) reader.Close(); // �������� �ݰ� false ��ȯ
            return false;
        }
    }

}
