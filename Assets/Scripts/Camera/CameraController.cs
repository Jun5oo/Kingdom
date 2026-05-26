using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Cinemachine 카메라 전환 컨트롤러. Awake 후 0.5초 뒤 startCamera → endCamera 우선순위를 교체한다.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineCamera startCamera;
    [SerializeField] CinemachineCamera endCamera;

    WaitForSeconds waitTime;

    void Awake()
    {
        startCamera.Priority = 10;
        endCamera.Priority = 0;

        waitTime = new WaitForSeconds(0.5f);

        StartCoroutine(CameraTransitionCoroutine()); 
    }

    // Cinemachine 카메라 이동 코루틴 
    IEnumerator CameraTransitionCoroutine()
    {
        yield return waitTime;

        startCamera.Priority = 0;
        endCamera.Priority = 10;
    }

    public void ShakeCamera()
    {

    }
}
