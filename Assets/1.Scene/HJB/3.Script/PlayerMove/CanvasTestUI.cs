using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTestUI : MonoBehaviour
{
    [Tooltip("Assign Main Panel so it can be turned on from Player:OnStartClient")]
    public RectTransform mainPanel;

    [Tooltip("Assign Players Panel for instantiating PlayerUI as child")]
    public RectTransform playersPanel;

    // static instance that can be referenced from static methods below.
    static CanvasTestUI instance;

    void Awake()
    {
        instance = this;
    }

    public static void SetActive(bool active)
    {
        instance.mainPanel.gameObject.SetActive(active);
    }

    public static RectTransform GetPlayersPanel() => instance.playersPanel;
    //public static RectTransform GetPlayersPanel() => instance.playersPanel;
}
