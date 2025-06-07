using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// 카메라 조작
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
}
