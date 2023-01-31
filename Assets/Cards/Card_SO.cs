using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class Card_SO : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite image;
    public int manaCost;
    public TargetType targetType;

    [Tooltip("Card effects to be activated when this card is played.")]
    public List<CardEffect> OnPlayEffects;
    [Tooltip("Card effects to be activated if this card kills its target.")]
    public List<CardEffect> OnKillEffects;

    
}

public enum TargetType{Player, Creature}

public enum EffectType{
    ChangeHealth, ChangeStrength, ChangeMana, DrawCard
}

[System.Serializable]
public class CardEffect{
    [SerializeField] public TargetType targetType;
    [SerializeField] public EffectType effectType;
    [SerializeField] public int strength;
    [SerializeField] public GameObject VFXPrefab;

    public void ActivateEffect(GameObject target)
    {
        switch(targetType)
        {
            case TargetType.Player:
                TargetPlayer(MonoBehaviour.FindObjectOfType<Player>());
                break;
            case TargetType.Creature:
                TargetCreature(target.GetComponent<Creature>());
                break;
        }
    }

    void TargetCreature(Creature creature)
    {
        switch(effectType)
        {
            case EffectType.ChangeHealth:
                creature.ChangeHealth(strength);
                break;
            case EffectType.ChangeStrength:
                creature.ChangeStrength(strength);
                break;
        }

        if (VFXPrefab) MonoBehaviour.Instantiate(VFXPrefab, creature.transform.position, Quaternion.identity);
    }

    void TargetPlayer(Player player)
    {
        switch(effectType)
        {
            case EffectType.ChangeMana:
                player.ChangeMana(strength);
                break;
            case EffectType.ChangeHealth:
                player.ChangeHealth(strength);
                break;
            case EffectType.DrawCard:
                for(int i = 0; i < strength; i++) player.DrawCard();
                break;
        }
    }
}