using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 페이즈 UI 항목(PhaseImage) 목록의 활성/비활성 전환 애니메이션을 제어하는 컨트롤러.
/// ActivateEffect(index)로 현재 페이즈를 활성화하고 나머지를 비활성/숨김 처리한다.
/// isFirstActivation이 true이면 너비 0(hideWidth)에서 시작하는 펼침 애니메이션을 실행한다.
/// </summary>
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
