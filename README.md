# 킹덤: Magic Chess

7x7 보드 위에서 진행하는 2인 전략 카드게임. 종족별 카드 효과와 배치 전략으로 상대의 왕을 먼저 처치하는 것이 승리 조건. 

- **개발 기간**: 2025.05 ~ 2025.09
- **팀 구성**: 기획 1, 개발 2, 아트 1 (61315 GameLab)
- **환경**: Unity

플레이 영상: https://youtu.be/8Pq8ChMHOH8

---

## 브랜치 안내

**카드 효과 시스템을 보시려면 `feature/0902`를 확인해 주세요.**

| 브랜치 | 내용 |
|---|---|
| `main` | 게임 전체 흐름이 동작하는 통합 브랜치. 다만 카드 효과는 `ActionType` / `PassiveType` enum과 팩토리 switch로 처리하는 초기 방식입니다. |
| `feature/0902` | ScriptableObject 조립식 카드 효과 시스템과 CSV 임포터가 들어간 최신 브랜치. 아래 설명은 이 브랜치 기준입니다. |

`main`의 `Utils/AbilityDefinition.cs`는 효과 시스템이 붙기 전에 카드 설명 텍스트를 임시로 채우기 위한 클래스이며, `feature/0902`에서는 사용하지 않습니다.

---

## 카드 효과 시스템 (`feature/0902`)

카드 고유 효과를 클래스마다 하드코딩하던 초기 구조를, 최소 단위로 쪼갠 ScriptableObject를 조립해 표현하는 방식으로 바꿨습니다.

```
AbilitySO
  └─ List<TriggeredEffect>
       ├─ Trigger            발동 시점 (Active / OnTurnStarted / OnTurnEnded / OnUnitDead)
       ├─ Target             대상 지정 방식
       ├─ triggerConditions  List<ConditionSO>     발동 조건
       ├─ targetConditions   List<ConditionSO>     대상 조건
       ├─ effects            List<EffectSO>        실제 수행할 효과
       └─ chainAbilities     List<TriggeredEffect> 연쇄 효과
```

- `EffectSO` (abstract) — `DamageSO`, `SummonSO`, `DestroySO`, `GainSO`
- `ConditionSO` (abstract) — `ConditionOnDeath`, `ConditionOnKilled`, `ConditionOwner`, `ConditionTag`, `ConditionTurn`, `ConditionGrid`
- `EffectContext` — 이벤트 시점의 정보를 키-값으로 전달. 하수인이 파괴되면서 참조가 끊겨 사망 효과가 발동하지 않던 문제를, `ObjectContext`에 파괴 시점 정보를 복사해두는 방식으로 해결했습니다.

주요 경로

```
Assets/Scripts/Effect/Ability/     AbilitySO, TriggeredEffect, Ability(런타임 실행)
Assets/Scripts/Effect/Effect/      EffectSO 및 구현체, EffectContext, ObjectContext
Assets/Scripts/Effect/Condition/   ConditionSO 및 구현체
Assets/Scripts/Utils/CSVReader.cs  CSV → ScriptableObject 임포터
```

---

## CSV 임포터

`Tools > Import Cards from CSV` 메뉴로 실행합니다.

- `Assets/CSV/CardData.csv` → `CardData` ScriptableObject 생성 **(동작)**
- `Assets/CSV/AbilityTable.csv` → 능력 데이터 **(미완, 코드 주석 처리 상태)**

능력 테이블 파싱은 구버전 `EffectData` 구조를 기준으로 작성되어 있어 현재 `AbilitySO` 구조와 맞지 않습니다. 카드 스탯만 CSV로 관리되고 능력은 인스펙터에서 직접 조립하고 있습니다.

---

## 알려진 한계

구조상 조립은 가능하지만, 실제로는 새 효과 하나에 여러 SO 에셋과 참조 연결이 필요해 제작 비용이 높습니다. 효과 조합은 데이터로 뺐지만 트리거 종류는 여전히 코드(`Ability.cs` 세 곳 + EventBus 이벤트 클래스) 수정이 필요하고, `ConditionSO`의 조건 판정이 조용히 통과할 수 있는 점, `TriggeredEffect`가 효과별로 유효 필드가 구분되지 않는 점, `FilterSO`가 아직 껍데기인 점이 남아 있습니다.
