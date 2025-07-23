using System.Collections.Generic;
using UnityEngine;

public class PhaseUIController : MonoBehaviour
{
    [SerializeField] private GameObject phaseParent;
    [SerializeField] private List<PhaseImage> phaseImages;
    [SerializeField] private float enableWidth = 400f;
    [SerializeField] private float disableWidth = 300f;
    [SerializeField] private float hideWidth = 0f;
    [SerializeField] private float disableAlpha = 0.5f;
    [SerializeField] private Vector3 currentPhaseScale = new Vector3(1.25f, 1.25f, 1.25f);

    // 현재 활성화된 페이즈 인덱스
    public int CurrentPhaseIndex { get; private set; } = 0;

    // 첫 활성화 여부
    public bool IsFirstActivation
    {
        get => isFirstActivation;
        set => isFirstActivation = value;
    }

    private bool isFirstActivation = true;

    private void Awake()
    {
        if (phaseParent != null)
        {
            foreach (var phaseImage in phaseImages)
            {
                phaseImage.UpdatePhaseColorAlpha(disableAlpha);
            }
        }
    }

    public void ActivateEffect(int index)
    {
        phaseParent.SetActive(true);

        CurrentPhaseIndex = index;

        float baseDelay = 0f;
        float stepDelay = 0.1f;

        for (int i = 0; i < phaseImages.Count; i++)
        {
            var phase = phaseImages[i];
            bool isCurrent = i == index;
            bool needsUpdate = isFirstActivation || phase.IsActivePhase != isCurrent;

            if (needsUpdate)
            {
                if (isFirstActivation)
                {
                    phase.SetRootWidth(hideWidth);
                }

                if (isCurrent)
                {
                }

                phase.DOKickPhase(
                    isCurrent,
                    isCurrent ? enableWidth : disableWidth,
                    isCurrent ? currentPhaseScale : Vector3.one,
                    baseDelay,
                    isInitial: isFirstActivation
                );

                baseDelay += stepDelay;
            }
        }

        isFirstActivation = false;
    }
}
