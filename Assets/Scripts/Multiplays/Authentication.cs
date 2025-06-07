using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class Authentication
{
    public async Task Initialize()
    {
        await InitializeUnityServices();
        await SignInAnonymously();
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            // Unity Services 초기화
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Serveices Initialized");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Sevices: {e}");
        }
    }

    private async Task SignInAnonymously()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("이미 로그인 상태입니다.");
            return;
        }

        try
        {
            // 이미 로그인 되어 있지 않은 경우에만 시도
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                // 익명으로 로그인
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"🧾 Signed in anonymously! PlayerID: {AuthenticationService.Instance.PlayerId}");
            }
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"❌ Authentication failed: {e}");
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"❌ Request failed: {e}");
        }
    }
}
