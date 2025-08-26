using System.Collections.Generic;

public class AbilityDefinition
{
    // 이 Branch에는 Effect가 구현되지 않았기에 임시로 사용하는 클래스 

    public Dictionary<(object, Race race), (string name, string definition)> definitionDict; 
    public AbilityDefinition()
    {
        definitionDict = new Dictionary<(object, Race race), (string name, string definition)> ();

        definitionDict.Add((ActionType.Resurrection, Race.Undead), ("일어나라", "데스카운트 2개를 소모하여 무덤의 언데드 아군을 부활 시킵니다."));
        definitionDict.Add((ActionType.DivineShield, Race.Celestial), ("여신의 성역", "여신 카운트 2개를 소모하여 아군 1명을 지정하고 성역을 부여합니다. 부여된 아군은 피격 시 피해량이 절반 감소하고 사망 면역을 얻습니다.."));

        definitionDict.Add((PassiveType.GainAbilityCoin, Race.Undead), ("언데드 킹의 저주", "적군 영웅이 사망할 경우, 데스 카운트 1을 획득합니다."));
        definitionDict.Add((PassiveType.SummonGraveyard, Race.Undead), ("언데드 킹의 저주", "아군 영웅이 사망할 경우, 해당 자리에 무덤을 소환합니다."));

        definitionDict.Add((PassiveType.GainAbilityCoin, Race.Celestial), ("여신의 영역", "적군 영웅이 사망할 경우, 여신 카운트 1을 획득합니다"));
        definitionDict.Add((PassiveType.DestroySelf, Race.Undead), (string.Empty, "두 번째 자신의 턴이 시작할 시, 파괴됩니다."));
    }
}
