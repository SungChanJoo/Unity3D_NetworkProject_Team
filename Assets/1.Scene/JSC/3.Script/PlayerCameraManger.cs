using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraManger : MonoBehaviour
{
    public static PlayerCameraManger Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
}
