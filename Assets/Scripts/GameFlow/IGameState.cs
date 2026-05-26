using Cysharp.Threading.Tasks;

/// <summary>
/// 게임 흐름 상태(TurnSelection, KingDraw, KingPlacement, MainPhase, GameOver)의 공통 인터페이스.
/// Enter()는 비동기로 실행되며 완료 후 다음 상태로 전환된다.
/// </summary>
public interface IGameState
{
    public UniTask Enter();
}
