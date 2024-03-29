using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private List<Image> player_Img;
    [SerializeField] private Text[] LobbyPlayerName;
    public static CreateRoomUI Instance = null;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void UpdatePlayerImage(int PlayerCount)
    {
        for (int i = 0; i < player_Img.Count; i++)
        {
            if (i < PlayerCount)
            {
                player_Img[i].gameObject.SetActive(true);
                LobbyPlayerName[i].text = $"{GameManager.Instance.PlayerList[i].playerName}";
            }
            else
            {
                player_Img[i].gameObject.SetActive(false);
                LobbyPlayerName[i].text = "";
            }
        }
    }

}