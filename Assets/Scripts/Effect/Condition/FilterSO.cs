using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FilterSO : ScriptableObject
{
    public virtual List<Vector2Int> filterTarget()
    {
        List < Vector2Int > position = new List<Vector2Int>();
        return position;

    }
}
