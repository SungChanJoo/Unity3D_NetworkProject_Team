using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using LitJson;

/*
 SQL 연결 단계
 0. 커넥션 생성
 1. 커넥션 오픈
 2.데이터 가지고 오기
 { 
    1. connection open 상황인지 확인
    2. reader 상태가 읽고 있는 상황인지 확인
        -한 쿼리문당 한개씩
    3.데이터를 다 읽었으면 Reader의 상태 확인 후 Close
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
        if (!File.Exists(path))// 그 경로에 파일이 있나요?
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
        //현재 MySqlConnection open 상태가 아니라면?
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
                Debug.Log("연결실패");
                return false;
            }
            SQLCommand = string.Format(@"SELECT User_Name FROM user_info WHERE User_Name = '{0}';", id);
            MySqlCommand cmd = new MySqlCommand(SQLCommand, Connection);
            Reader = cmd.ExecuteReader();
            //Reader 읽은 데이터가 1개 이상 존재해?
            if (Reader.HasRows)
            {
                //읽은 데이터를 하나씩 나열합니다.
                while (Reader.Read())
                {
                    /*
                     삼항연산자
                    */
                    string name = (Reader.IsDBNull(0)) ? string.Empty : (string)Reader["User_Name"].ToString();
                    if (!name.Equals(string.Empty))
                    {
                        //Id 가 이미 존재하는 상황
                        //회원가입 실패
                        break;
                    }
                }//end while
            }//end if
            else
            {
                if (!Reader.IsClosed) Reader.Close();

                //회원가입 성공
                SQLCommand = string.Format(@"INSERT INTO user_info(User_Name, User_Password, User_Nickname) VALUES('{0}','{1}','{2}');", id, password, nickname);
                MySqlCommand joinCmd = new MySqlCommand(SQLCommand, Connection);
                if (joinCmd.ExecuteNonQuery() == 1)
                {
                    //정상적으로 data를 불러온 상황
                    Info = new user_info(id, password, nickname);
                }
                else
                {
                    Debug.Log("인서트 실패");
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
        //직접적으로 DB에서 데이터를 가지고 오는 메소드
        //조회되는 데이터가 없다면 false
        //조회가 되는 데이터가 있다면 true값을 던지는데
        //위에서 선언한 info에다가 담은 다음에 던짐

        /*
            1. connection open 상황인지 확인
            2. reader 상태가 읽고 있는 상황인지 확인
                -한 쿼리문당 한개씩
            3.데이터를 다 읽었으면 Reader의 상태 확인 후 Close
         */

        try
        {
            if (!ConnectionCheck(Connection))
            {
                Debug.Log("연결실패");
                return false;
            }

            string SQLCommand = string.Format(@"SELECT User_Name, User_Password FROM user_info
                                                WHERE User_Name='{0}' AND User_Password = '{1}';", id, password);

            MySqlCommand cmd = new MySqlCommand(SQLCommand, Connection);
            Reader = cmd.ExecuteReader();
            //Reader 읽은 데이터가 1개 이상 존재해?
            if (Reader.HasRows)
            {
                //읽은 데이터를 하나씩 나열합니다.
                while (Reader.Read())
                {
                    /*
                     삼항연산자
                    */
                    string name = (Reader.IsDBNull(0)) ? string.Empty : (string)Reader["User_Name"].ToString();
                    string pass = (Reader.IsDBNull(1)) ? string.Empty : (string)Reader["User_Password"].ToString();
                    string nick = (Reader.IsDBNull(2)) ? string.Empty : (string)Reader["User_Nickname"].ToString();

                    if (!name.Equals(string.Empty) || !pass.Equals(string.Empty) || !nick.Equals(string.Empty))
                    {
                        //정상적으로 data를 불러온 상황
                        Info = new user_info(name, pass, nick);

                        //Reader가 닫혀있니?
                        if (!Reader.IsClosed) Reader.Close();
                        return true;
                    }
                    else
                    {
                        //로그인 실패
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
