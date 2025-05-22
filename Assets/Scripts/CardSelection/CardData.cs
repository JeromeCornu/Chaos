using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite cardImage;
    [TextArea] public string description;

    public List<StatModifier> statModifiers = new();
}
