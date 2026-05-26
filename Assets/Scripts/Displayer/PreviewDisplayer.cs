using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 마우스 오버 시 카드/토큰 정보를 프리뷰 UI에 표시하는 컴포넌트.
/// HoverSystem의 onHoverStart/onHoverExit 이벤트를 구독하며,
/// 상대 카드(원격 플레이어 소유 Card)는 프리뷰를 표시하지 않는다.
/// </summary>
public class PreviewDisplayer : MonoBehaviour
{
    [SerializeField] Preview previewUI;

    PlayerManager playerManager;

    HoverSystem hoverSystem;
    CardTextureLoader loader;

    public void Start()
    {
        playerManager = ServiceLocator.Get<PlayerManager>(); 
        hoverSystem = ServiceLocator.Get<HoverSystem>();
        loader = ServiceLocator.Get<CardTextureLoader>();

        Subscribe();
    }

    void Subscribe()
    {
        Unsubscribe();
        hoverSystem.onHoverStart += OnHoverHandler;
        hoverSystem.onHoverExit += UnDisplay;
    }

    void Unsubscribe()
    {
        hoverSystem.onHoverStart -= OnHoverHandler;
        hoverSystem.onHoverExit -= UnDisplay;
    }

    void OnHoverHandler(BaseObject baseObject)
    {
        Display(baseObject).Forget(); // UniTask: 결과를 기다리지 않고 Fire-and-Forget 실행
    }

    /// <summary>
    /// 텍스처를 비동기 로드한 뒤 프리뷰 UI를 갱신하고 표시한다.
    /// 상대방 카드(Card 타입 + 원격 소유)는 무시한다.
    /// </summary>
    async UniTask Display(BaseObject baseObject)
    {
        if (baseObject == null)
            return;

        if (baseObject is Card && baseObject.OwnerID != playerManager.Local.PlayerID)
            return;

        UnDisplay();

        VisualTexture textures = await loader.LoadAllTextures(baseObject.Data);

        previewUI.UpdatePreview(baseObject, textures);
        previewUI.gameObject.SetActive(true);
        previewUI.Show();
    }

    void UnDisplay() => previewUI.Hide();
}
