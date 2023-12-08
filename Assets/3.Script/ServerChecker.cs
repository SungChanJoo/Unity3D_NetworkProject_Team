using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using kcp2k;
using LitJson;
using System;
using System.IO;



public enum Type
{
    Empty = 0,
    Server,
    Client
}

//IP�� �ٲ�� �ֱ� ������ Json���� ����
public class Item
{
    //�������� Ű������ Json ���� ����
    //������ �� ����~!
    public string License;
    public string ServerIP;
    public string Port;

    public Item(string licenceIndex, string ipValue, string port)
    {
        License = licenceIndex;
        ServerIP = ipValue;
        Port = port;
    }
}

public class ServerChecker : MonoBehaviour
{
    public Type type;

    private NetworkManager manager;
    private KcpTransport kcp;

    private string path = string.Empty;
    public string ServerIp { get; private set; }
    public string ServerPort { get; private set; }


    private void Awake()
    {
        if (path.Equals(string.Empty))
        {
            path = Application.dataPath + "/License";
        }
        if (!File.Exists(path))//�����˻�
        {
            Directory.CreateDirectory(path);
        }
        if (!File.Exists(path + "/License.json"))//���ϰ˻�
        {
            DefaultData(path);
        }
        //manager = GetComponent<NetworkManager>();
        manager = RoomManager.singleton;
        kcp = (KcpTransport)manager.transport;
    }

    private void DefaultData(string path)
    {
        List<Item> item = new List<Item>();
        item.Add(new Item("1", "127.0.0.1", "7777"));

        JsonData data = JsonMapper.ToJson(item);
        File.WriteAllText(path + "/License.json", data.ToString());

    }

    private Type LicenseType()
    {
        Type type = Type.Empty;

        try
        {
            string jsonString = File.ReadAllText(path + "/License.json");
            JsonData itemData = JsonMapper.ToObject(jsonString);
            string strType = itemData[0]["License"].ToString();
            string strServerIp = itemData[0]["ServerIP"].ToString();
            string strPort = itemData[0]["Port"].ToString();

            ServerIp = strServerIp;
            ServerPort = strPort;

            type = (Type)Enum.Parse(typeof(Type), strType);

            manager.networkAddress = ServerIp;
            kcp.port = ushort.Parse(ServerPort);

            return type;


        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return Type.Empty;
        }
    }

    private void Start()
    {
        type = LicenseType();

        if (type.Equals(Type.Server))
        {
            Start_Server();
        }
        else
        {
            Debug.Log("��ŸƮ! Client");
            Start_Client();
        }

    }

    public void Start_Server()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("WebGL cannot be Server");
        }
        else
        {
            manager.StartServer();
            Debug.Log($"{manager.networkAddress} StartServer....");
            NetworkServer.OnConnectedEvent += (NetworkConnectionToClient) =>
            {
                Debug.Log($"New client Connect : {NetworkConnectionToClient.address}");
            };
            NetworkServer.OnDisconnectedEvent += (NetworkConnectionToClient) =>
            {
                Debug.Log($"client DisConnect : {NetworkConnectionToClient.address}");
            };
            //Debug.Log("StartServer(), �÷��̾��" + ServerManager.Instance.PlayerNum);

        }
    }


    public void Start_Client()
    {
        manager.StartClient();
        Debug.Log($"{manager.networkAddress} : Startclient...");
    }
    public void Start_Host()
    {
        manager.StartHost();
        Debug.Log($"{manager.networkAddress} : StartHost...");
    }

    private void OnApplicationQuit()
    {
        if (NetworkClient.isConnected)
        {
            manager.StopClient();
        }

        if (NetworkServer.active)
        {
            manager.StopServer();
        }
    }


}
