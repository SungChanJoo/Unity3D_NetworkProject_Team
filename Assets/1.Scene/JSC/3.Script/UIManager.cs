using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _joinUI;
    public void SetActiveBtn()
    {
        _joinUI.SetActive(true);
    }
}
