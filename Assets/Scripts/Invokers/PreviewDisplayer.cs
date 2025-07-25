using Cysharp.Threading.Tasks;
using UnityEngine;

public class PreviewDisplayer : MonoBehaviour
{
    [SerializeField] PreviewUI previewUI;
    SelectionSystem selectionSystem;
    TextureLoader loader; 

    public void Start()
    {
        selectionSystem = ServiceLocator.Get<SelectionSystem>();
        loader = ServiceLocator.Get<TextureLoader>();

        Subscribe(); 
    }

    void Subscribe()
    {
        Unsubscribe();
        selectionSystem.onSelected += OnSelectedHandler;
        selectionSystem.onDeselected += UnDisplay; 
    }

    void Unsubscribe()
    {
        selectionSystem.onSelected -= OnSelectedHandler;
        selectionSystem.onDeselected -= UnDisplay;
    }

    void OnSelectedHandler(BaseObject baseObject)
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

        previewUI.OnUpdate(baseObject, textures); 
        previewUI.gameObject.SetActive(true); 

    }

    void UnDisplay() => previewUI.gameObject.SetActive(false);
}
