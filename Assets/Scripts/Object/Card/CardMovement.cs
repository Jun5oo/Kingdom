using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// 카드의 DOTween 이동 애니메이션을 담당한다.
/// BaseMovement를 상속하며, 초기 PRS를 (0, 90도 X 회전, 스케일 1)로 설정한다.
/// </summary>
public class CardMovement : BaseMovement
{
    public override void Init()
    {
        Vector3 position = Vector3.zero;
        Vector3 eulerAngles = new Vector3(90f, 0f, 0f);
        Quaternion rotation = Quaternion.Euler(eulerAngles);
        Vector3 scale = Vector3.one;

        PRS = new PRS(position, rotation, scale);
    }
}
