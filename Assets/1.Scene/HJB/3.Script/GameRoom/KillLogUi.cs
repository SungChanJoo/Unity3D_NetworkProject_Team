using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class KillLogUi : NetworkBehaviour
{
    [Header("KillLog")]
    [SerializeField] private Text killLogText;

    internal static string localPlayerName;

    internal static readonly Dictionary<NetworkConnectionToClient, string> connNames =
        new Dictionary<NetworkConnectionToClient, string>();


    public override void OnStartServer()
    {
        connNames.Clear();
    }

    public override void OnStartClient()
    {
        killLogText.text = "";
    }
}
