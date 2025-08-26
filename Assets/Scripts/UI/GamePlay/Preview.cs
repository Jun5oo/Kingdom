using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] GridLayoutGroup layout;
    [SerializeField] List<GameObject> layoutChildren; 

    CancellationTokenSource cts;

    AbilityDefinition abilityDefinition = new AbilityDefinition();

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        this.gameObject.SetActive(false);
    }
    public void UpdatePreview(BaseObject baseObject, VisualTexture textures)
    {
        if (baseObject == null)
            return;

        art.texture = textures.Art;
        background.texture = textures.Background;
        frame.texture = textures.Frame;

        cardName.text = baseObject.Name;

        Dictionary<string, string> abilityDict = new Dictionary<string, string>();

        foreach(var action in baseObject.Data.Action)
        {
            if(abilityDefinition.definitionDict.TryGetValue((action, baseObject.Race), out var value))
            {
                (string name, string definition) = value;

                if (!abilityDict.ContainsKey(name))
                    abilityDict.Add(name, definition); 
            }
        }

        foreach(var passive in baseObject.Data.Passive)
        {
            if (abilityDefinition.definitionDict.TryGetValue((passive, baseObject.Race), out var value))
            {
                (string name, string definition) = value;

                if (!abilityDict.ContainsKey(name))
                    abilityDict.Add(name, definition);
            }
        }

        if (abilityDict.Count > 0)
        {
            int count = 0; 

            description.text = string.Empty;

            foreach (var entry in abilityDict)
            {
                if (entry.Key == string.Empty)
                {
                    description.text += entry.Value;
                    count++;
                    continue; 
                }

                description.text += $"<b>{entry.Key}</b>"; 
                
                if(count < abilityDict.Count - 1)
                    description.text += ", "; 

                TextMeshProUGUI text = layoutChildren[count].GetComponentInChildren<TextMeshProUGUI>();
                text.text = string.Empty; 

                text.text += $"<b>{entry.Key}</b>";
                text.text += "\n";
                text.text += entry.Value;

                layoutChildren[count].SetActive(true);

                count++;
            }

        }

        else
        {
            description.text = string.Empty;

            foreach(var gob in layoutChildren)
            {
                TextMeshProUGUI text = gob.GetComponentInChildren<TextMeshProUGUI>();
                text.text = string.Empty; 
                gob.SetActive(false);

            }
        }

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

        foreach(var gob in layoutChildren)
        {
            TextMeshProUGUI text = gob.GetComponentInChildren<TextMeshProUGUI>();
            text.text = string.Empty;
            gob.gameObject.SetActive(false);
        }
  
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

            while (t < duration)
            {
                ct.ThrowIfCancellationRequested();
                t += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(start, dest, t / duration);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

        }
        catch (OperationCanceledException)
        {

        }

        finally
        {
            cts = null;
        }
    }
}
