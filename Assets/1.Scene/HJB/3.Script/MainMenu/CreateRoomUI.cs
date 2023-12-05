using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private List<Image> player_Img;

    [SerializeField] private List<Button> playerCountButton;  

    

    private CreateRoomData roomData;

    private void Start()
    {
        roomData = new CreateRoomData() { MaxPlayerCout = 8 };
    }
    private void UpdatePlayerImage()
    {
        int PlayerCount = roomData.MaxPlayerCout;        

        for (int i = 0; i < player_Img.Count; i++)
        {
            if (i<roomData.MaxPlayerCout)
            {
                player_Img[i].gameObject.SetActive(true);
            }
            else
            {
                player_Img[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateMaxPlayerCount(int count)
    {
        roomData.MaxPlayerCout = count;
        
        UpdatePlayerImage();
    } 
}
public class CreateRoomData
{
    public int MaxPlayerCout;
}
