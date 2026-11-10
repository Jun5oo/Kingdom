# 코드 컨벤션

새로 작성하거나 수정하는 C# 코드에 적용합니다. 이 문서에 없는 서식은 `.editorconfig`를 따릅니다.

## 이름

| 대상 | 규칙 | 예시 |
|---|---|---|
| 클래스, 구조체, 열거형 | PascalCase | `GameEngine`, `RangeDirection` |
| 인터페이스 | `I` + PascalCase | `IMovable` |
| 메서드 | PascalCase | `EndTurn` |
| 프로퍼티 | PascalCase | `CurrentHP`, `Board` |
| 필드 | `_` 없이 camelCase | `cardDatabase` |
| 매개변수, 지역 변수 | camelCase | `targetX`, `levelData` |

### 메서드 접두사

- `Can~`: 실행 가능 여부만 확인합니다. 상태를 변경하지 않습니다.
- `Try~`: 실패할 수 있는 실행이나 조회입니다. 성공 여부를 `bool`로 반환합니다.

```csharp
public bool CanMoveUnit(int playerId, int unitId, int targetX, int targetY, out string reason)
public bool TryGetCard(string cardId, out UnitCardData card)
```

## 중괄호

`if`, `for`, `foreach`, `while`의 본문이 한 줄이면 중괄호를 생략합니다. 본문은 다음 줄에 들여써서 작성합니다.

```csharp
if (rule == null)
    return false;

foreach (PlayerState player in players)
    player.ActionResource = 0;
```

본문이 두 줄 이상이면 중괄호를 사용하고, 여는 중괄호는 새 줄에 둡니다.

```csharp
if (rule == null)
{
    reason = "이동 규칙이 없습니다.";
    return false;
}
```

## 필드와 프로퍼티

- 접근 제한자를 항상 명시합니다.
- 외부에 공개할 값은 필드 대신 프로퍼티로 노출합니다.
- 외부에서 수정하면 안 되는 컬렉션은 읽기 전용 타입으로 노출합니다.
- Inspector에서 설정할 값은 `[SerializeField] private` 필드와 읽기 전용 프로퍼티로 작성합니다.
- 카드 JSON에서 읽을 값은 `[JsonProperty]`를 붙인 `{ get; private set; }` 프로퍼티로 작성합니다. JSON 항목 이름은 프로퍼티 이름의 camelCase입니다(`BaseCP` → `baseCP`). 컬렉션은 `IReadOnlyList<T>` 타입으로 선언합니다. 클래스 안에서만 쓰는 값은 `[JsonProperty] private` 필드로 둡니다.

```csharp
[SerializeField] private string cardId;

public string CardId => cardId;
```

## 데이터와 UI 분리

| 역할 | 담당 | 하는 일 |
|---|---|---|
| 원본 데이터 | 카드 JSON, `CardDefinition` | 카드의 변하지 않는 정의를 보관합니다. |
| 런타임 상태 | `GameState`, `BoardState`, `UnitState` | 진행 중인 게임의 값을 보관합니다. |
| 규칙 처리 | `GameEngine`, `MovementValidator`, `CombatResolver`, `EffectProcessor` | 행동을 검증하고 상태를 변경하며, 결과를 이벤트로 남깁니다. |
| UI | `BoardView`, `UnitView`, `GamePresenter` | 엔진이 돌려준 이벤트를 화면에 재생하고 입력을 명령으로 전달합니다. |

- UI는 상태를 직접 변경하지 않고, 엔진의 `Submit`으로 명령을 보냅니다. 가능한 행동은 `GetAvailableCommands`로 조회합니다.
- 규칙 처리와 상태 코드는 UI 클래스를 참조하지 않습니다.
- 규칙 처리와 상태 코드는 Unity 참조를 끈 별도 asmdef에 둡니다. 이후 .NET 서버로 옮기기 위한 제약입니다.
- 카드 정의는 JSON 파일로 작성합니다. 그림 같은 Unity 자원은 클라이언트 코드에서만 다룹니다.
- 규칙 처리 코드는 객체 참조 대신 ID로 유닛, 플레이어, 카드를 가리킵니다.
- 게임 중 변하는 값은 ScriptableObject가 아니라 런타임 상태에 저장합니다.
- 같은 값을 여러 곳에 중복 저장하지 않습니다. 예를 들어 유닛의 위치는 `BoardState`에서만 관리합니다.

## 실패 처리와 상태 변경

- 게임 규칙상 일어날 수 있는 실패는 `bool`과 `out string reason`으로 반환합니다. 결과 객체를 돌려주는 메서드(예: `Submit`)는 실패 사유를 결과의 `Reason`에 담습니다.
- `reason`에는 실패 사유를 한글로 작성합니다.
- 잘못된 인수나 코드 오류처럼 게임 진행 중 일어나면 안 되는 문제는 예외를 던집니다.
- 실패한 행동은 상태를 변경하지 않습니다. 검증을 먼저 끝낸 뒤 상태를 변경합니다.

## 기타

- 지역 변수는 `var` 대신 타입을 명시합니다. 대상 타입이 명확하면 `new()`를 사용합니다.
- 주석은 한글로 작성하고, 코드만 봐서는 알 수 없는 의도와 제약을 설명합니다.
- 클래스 주석은 클래스의 역할만 최대한 짧게 한 줄로 작성합니다.
