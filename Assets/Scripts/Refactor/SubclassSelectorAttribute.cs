using System;
using UnityEngine;

namespace Refactor
{
    // [SerializeReference] 필드/리스트 위에 함께 붙이면, 인스펙터에 구현 타입 선택 드롭다운을 그린다.
    // Inspector에 표시되지 않는 문제 대응 
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SubclassSelectorAttribute : PropertyAttribute { }
}
