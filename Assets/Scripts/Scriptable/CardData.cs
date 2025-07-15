using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Scriptable")]
public abstract class CardData : ScriptableObject
{
    [SerializeField] string cardName;
    [SerializeField] Sprite sprite;
    [SerializeField] string description;

    // CardArt의 경우 CardShader에서 사용. (추후에는 Addressable asset에 등록시켜줄 예정, CardData에는 CardID, Name, Description만 저장할 예정) 
    // 또는 Texture만을 CardData에 저장하고 Sprite로 변환 후 사용할 예정. 
    [SerializeField] Texture2D cardArt;

    public string Name {  get { return cardName; } }
    public Sprite Sprite { get { return sprite; } }
    public string Description { get { return description; } }
    public Texture2D CardArt { get {  return cardArt; } }

}
