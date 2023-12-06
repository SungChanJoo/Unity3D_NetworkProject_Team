using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CM_Find_Player : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;
    private void Awake()
    {
        TryGetComponent(out freeLookCamera);

        Transform followTarget = GameObject.FindWithTag("Player").transform;
        Transform lookAtTarget = GameObject.FindWithTag("Player").transform;

        // Follow�� LookAt�� ����
        freeLookCamera.m_Follow = followTarget;
        freeLookCamera.m_LookAt = lookAtTarget;
    }

}
