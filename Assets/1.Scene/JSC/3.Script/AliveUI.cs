using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AliveUI : MonoBehaviour
{
    public static AliveUI Instance = null;
    [SerializeField] private Text aliveUI;


    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IsAliveUI(int alive)
    {
        aliveUI.text = $"{alive}";
    }
}
