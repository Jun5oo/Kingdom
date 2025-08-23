using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    const float FADE_DURATION = 0.2f;
    const float FADE_ALPHA = 1f; 

    [SerializeField] RawImage art;
    [SerializeField] RawImage background;
    [SerializeField] RawImage frame; 

    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement;

    [SerializeField] RectTransform viewPort;

    [SerializeField] CanvasGroup canvasGroup;

    CancellationTokenSource cts; 

    void Awake()
    {
        if(canvasGroup == null)
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f; 
        this.gameObject.SetActive(false);
    }

    public void UpdatePreview(BaseObject baseObject, VisualTexture textures)
    {
        art.texture = textures.Art;
        background.texture = textures.Background; 
        frame.texture = textures.Frame;

        cardName.text = baseObject.Name;
        
        description.enableAutoSizing = false;

        description.text = baseObject.Description;
        
        float width = Mathf.Max(1f, viewPort.rect.width);
        float preferredHeight = description.GetPreferredValues(description.text, width, 0f).y; 

        if (viewPort.rect.height < preferredHeight)
            TopAlignment();
        else
            CenterAlignment();

        cp.enabled = true;
        movement.enabled = true;

        switch (baseObject)
        {
            case Card card:
                cp.text = card.CP.ToString();
                movement.text = card.Movement.ToString();
                break;
            case Token token:
                cp.text = token.CP.ToString();
                movement.text = token.Movement.ToString();
                break;

            default:
                cp.enabled = false;
                movement.enabled = false;
                break;
        }
    }
    void SetAlignment(float anchorY, float pivotY)
    {
        var rt = description.rectTransform;
        rt.anchorMin = new Vector2(rt.anchorMin.x, 0.5f);
        rt.anchorMax = new Vector2(rt.anchorMax.x, 0.5f);
        rt.pivot = new Vector2(rt.pivot.x, 0.5f);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f);
    }

    void CenterAlignment() => SetAlignment(0.5f, 0.5f);
    void TopAlignment() => SetAlignment(1f, 1f);  

    public void Show()
    {
        if (canvasGroup == null)
            return;

        FadeIn(FADE_ALPHA, FADE_DURATION).Forget(); 
    }
    
    public void Hide()
    {
        cts?.Cancel();
            
        canvasGroup.alpha = 0f;
        this.gameObject.SetActive(false);
    }

    async UniTaskVoid FadeIn(float dest, float duration)
    {
        canvasGroup.alpha = 0;
        
        cts?.Cancel();

        if (cts == null)
            cts = new CancellationTokenSource();

        var ct = cts.Token; 
        
        try
        {
            if (!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);

            float start = canvasGroup.alpha;
            float t = 0f; 

            while(t < duration)
            {
                ct.ThrowIfCancellationRequested();
                t += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(start, dest, t / duration);
                await UniTask.Yield(PlayerLoopTiming.Update, ct); 
            }

        }
        catch(OperationCanceledException)
        {

        }

        finally
        {
            cts = null;
        }
    }
}
