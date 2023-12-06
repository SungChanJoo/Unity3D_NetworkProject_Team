using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void onClickEnterRoomButton()
    {
        Debug.Log("EnterRoom");

    }
    public void onClickQuickOut()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Á¾·á");
#else
        Application.Quit();
#endif
    }
}
