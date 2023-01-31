using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Creature : MonoBehaviour
{
    public Creature_SO creatureType;
    public int health;
    public int strength;
    public Image image;
    public TextMeshProUGUI displayHealth;
    public TextMeshProUGUI displayStrength;
    public List<PassiveEffect> activePassiveEffects = new List<PassiveEffect>();

    public void LoadCreature_SO(Creature_SO creatureType){
        this.creatureType = creatureType;
        this.health = creatureType.health;
        this.strength = creatureType.strength;

        image.sprite = creatureType.image;
        displayHealth.text = health.ToString();
        displayStrength.text = strength.ToString();
    }
    
    public void SetHealth(int amount){
        health = amount;
        displayHealth.text = health.ToString();
    }

    public void ChangeHealth(int amount)
    {
        health += amount;
        displayHealth.text = health.ToString();
    }

    public void SetStrength(int amount){
        strength = amount;
        displayStrength.text = strength.ToString();
    }

    public void ChangeStrength(int amount)
    {
        strength += amount;
        displayStrength.text = strength.ToString();
    }

    public void AddPassiveEffect(PassiveEffect passiveEffect)
    {
        activePassiveEffects.Add(passiveEffect);
    }
}
