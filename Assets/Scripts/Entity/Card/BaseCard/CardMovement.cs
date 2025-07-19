using DG.Tweening;
using System;
using UnityEngine;

public class CardMovement : EntityMovement
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
