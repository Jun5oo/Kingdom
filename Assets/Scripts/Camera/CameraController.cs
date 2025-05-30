using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

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

    IEnumerator CameraTransitionCoroutine()
    {
        yield return waitTime;

        startCamera.Priority = 0;
        endCamera.Priority = 10;
    }
}
