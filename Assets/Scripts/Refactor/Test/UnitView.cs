using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class UnitView : MonoBehaviour
    {
        [SerializeField] private Image artwork;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text cpText;

        private int cp;
        private int maxHP;

        public int UnitId { get; private set; }
        public int OwnerId { get; private set; }
        public bool IsConfigured => artwork != null && nameText != null && cpText != null;

        public void Bind(UnitState unit, CardDefinition card)
        {
            // 클릭은 부모 칸의 Button으로 전달한다.
            foreach (Graphic graphic in GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = false;
            UnitId = unit.Id;
            OwnerId = unit.OwnerId;
            cp = unit.CP;
            maxHP = unit.MaxHP;
            // shortcut: 그림이 있는 카드가 없어 숨긴다, 그림이 생기면 카드 ID → Sprite 연결 추가
            artwork.enabled = false;
            nameText.text = $"P{unit.OwnerId} · {card.CardName}";
            nameText.color = unit.OwnerId == 1
                ? new Color(0.4f, 0.75f, 1f)
                : new Color(1f, 0.55f, 0.4f);
            SetHP(unit.CurrentHP);
        }

        public void SetHP(int currentHP)
        {
            // 기존 프리팹의 능력치 텍스트에 함께 표시한다.
            cpText.text = $"CP {cp}\nHP {currentHP}/{maxHP}";
        }
    }
}
