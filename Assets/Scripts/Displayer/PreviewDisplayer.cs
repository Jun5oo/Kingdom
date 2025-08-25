using Cysharp.Threading.Tasks;
using UnityEngine;

public class PreviewDisplayer : MonoBehaviour
{
    // 카드 Preview Displayer, 카드의 정보를 프리뷰로 보여줌 
    [SerializeField] Preview previewUI;

    HoverSystem hoverSystem; 
    CardTextureLoader loader; 

    public void Start()
    {
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
        // Fire and Forget 
        Display(baseObject).Forget(); 
    }

    async UniTask Display(BaseObject baseObject)
    {
        if (baseObject == null)
            return;

        UnDisplay();

        VisualTexture textures = await loader.LoadAllTextures(baseObject.Data);

        previewUI.UpdatePreview(baseObject, textures); 
        previewUI.gameObject.SetActive(true);
        previewUI.Show(); 

    }

    void UnDisplay() => previewUI.Hide(); 
}
