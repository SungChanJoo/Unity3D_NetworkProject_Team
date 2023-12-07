using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTestUI : MonoBehaviour
{

    [Header("Player Components")]
    public Image image;

    [Header("Child Text Objects")]
    public Text playerNameText;
    public Text playerDataText;

    public void SetLocalPlayer()
    {
        image.color = new Color(1f, 1f, 1f, 0.1f);
    }

    public void OnPlayerNumberChanged(byte newPlayerNumber)
    {
        playerNameText.text = string.Format("Player {0:00}", newPlayerNumber);
    }

    public void OnPlayerColorChanged(Color32 newPlayerColor)
    {
        playerNameText.color = newPlayerColor;
    }

    public void OnPlayerDataChanged(ushort newPlayerData)
    {
        // Show the data in the UI
        playerDataText.text = string.Format("Data: {0:000}", newPlayerData);
    }
}

