using System;
using System.IO;
using UnityEngine;

namespace Refactor
{
    // StreamingAssets/Cards의 카드 JSON을 읽어 CardDatabase를 만든다.
    public static class CardFiles
    {
        // shortcut: File API로 읽으므로 PC 빌드 전용, Android/WebGL을 지원할 때 UnityWebRequest로 교체
        public static string Directory => Path.Combine(Application.streamingAssetsPath, "Cards");

        // 데이터 오류는 게임을 시작할 수 없는 문제이므로 로그를 남기고 false를 반환한다.
        public static bool TryCreateDatabase(UnityEngine.Object logContext, out CardDatabase database)
        {
            try
            {
                database = new CardDatabase(CardJsonLoader.LoadDirectory(Directory));
                return true;
            }
            catch (Exception exception) when (exception is IOException or InvalidDataException or ArgumentException)
            {
                Debug.LogError(exception.Message, logContext);
                database = null;
                return false;
            }
        }
    }
}
