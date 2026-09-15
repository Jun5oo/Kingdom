using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor
{
    // 카드 에셋 내부에 저장되는 등급별 정의. 런타임 상태와 분리한다.
    [Serializable]
    public sealed class UnitLevelData
    {
        [SerializeField, Min(1)] private int level = 1;
        [SerializeField, Min(0)] private int baseCP;

        [SerializeField] private RangeData movement = new();
        [SerializeField] private RangeData attack = new();

        // 해당 등급의 전체 효과 목록. 이전 등급에서 자동으로 상속하지 않는다.
        [SerializeField] private List<EffectData> effects = new();

        public int Level => level;
        public int BaseCP => baseCP;
        public RangeData Movement => movement;
        public RangeData Attack => attack;
        public IReadOnlyList<EffectData> Effects => effects;
    }
}
