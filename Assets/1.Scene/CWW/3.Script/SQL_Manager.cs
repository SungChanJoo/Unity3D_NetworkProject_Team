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
     sql 서버오픈
     connection 생성 -> 오픈
     데이터 조회(가져오기)
    1. 오픈상태 확인
    2. 리더 상태가 읽고 있는 상황인지 확인 -한쿼리당 한개
    3. 다읽으면 확인후 close
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
        if (!File.Exists(path)) //그 경로에 파일이 없다면
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
        //현재 MySqlConnection open이 아니라면?
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
        //직접적으로DB에서 데이터를 가지고 오는 메소드
        //조회되는 데이터가 없다면 False
        //조회가 되는 데이터가 있다면 true 값을 던지는 데 
        //위에서 선언한 info에다가 담은 다음에 던질꺼임

        try
        {
            //1.connction open 확인
            if (!connection_check(connection))
            {
                return false;
            }
            string SQL_command = string.Format(@"SELECT USER_NAME,USER_PASSWORD FROM USER_INFO
                    WHERE USER_NAME = '{0}' AND User_password = '{1}';", id, password);
            MySqlCommand cmd = new MySqlCommand(SQL_command, connection);
            reader = cmd.ExecuteReader();
            //Reader 읽은 데이터가 1개이상 존재하니?
            if (reader.HasRows)
            {
                //읽은 데이터를 하나씩 나열
                while (reader.Read())
                {
                    //삼항연상자
                    string name = (reader.IsDBNull(0)) ? string.Empty : (string)reader["USER_NAME"].ToString();
                    string pass = (reader.IsDBNull(1)) ? string.Empty : (string)reader["USER_PASSWORD"].ToString();
                    if (!name.Equals(string.Empty) || !name.Equals(string.Empty))
                    {
                        //정상적으로 Data를 불러온 상황
                        info = new user_info(name, pass);
                        if (!reader.IsClosed) reader.Close();
                        return true;
                    }
                    else // 로그인실패
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
