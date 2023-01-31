using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Player player;
    public Board board;
    public GameObject creaturePrefab;
    public Creature_SO creatureType;
    public List<Creature> creatures = new List<Creature>();

    void SpawnCreature(Creature_SO creatureType){
        GameObject newCreature = board.NewBoardItem(creaturePrefab);

        Creature creature = newCreature.GetComponent<Creature>();
        creature.LoadCreature_SO(creatureType);
        creatures.Add(creature);

        creature.AddPassiveEffect(new PassiveEffect(1, true));
    }

    void OnTurnPhaseChange(TurnPhase newTurnPhase)
    {
        CleanUpDead();
    }

    public void CleanUpDead(){
        List<Creature> deadCreatures = new List<Creature>();
        for(int i = 0; i < creatures.Count; i++){
            if (creatures[i].health <= 0) deadCreatures.Add(creatures[i]);
        }

        while(deadCreatures.Count > 0){
            DestroyCreature(deadCreatures[0]);
            deadCreatures.RemoveAt(0);
        }
    }

    public void DestroyCreature(Creature creature){
        board.RemoveBoardItem(creature.gameObject.GetComponent<BoardItem>());
        creatures.Remove(creature);
    }

    private void Awake()
    {
        TurnManager.OnPhaseChanged.AddListener(OnTurnPhaseChange);
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        SpawnCreature(creatureType);
        SpawnCreature(creatureType);
        SpawnCreature(creatureType);
    }

    public void StartTurn()
    {
        StartCoroutine(TurnSequence());
    }

    IEnumerator TurnSequence()
    {
        if (creatures.Count < 3){
            SpawnCreature(creatureType);
        }
        
        yield return new WaitForSeconds(0.5f);

        foreach(Creature c in creatures)
        {
            bool attack = true;

            List<PassiveEffect> removeEffects = new List<PassiveEffect>();
            foreach(PassiveEffect p in c.activePassiveEffects){
                p.turnsRemaining--;
                if (p.turnsRemaining <= 0) removeEffects.Add(p);

                if (p.stopAttack) attack = false;
            }

            for(int i = 0; i < removeEffects.Count; i++) c.activePassiveEffects.Remove(removeEffects[i]);


            if (attack)
            {
                c.GetComponent<BoardItem>().offset = Vector3.down * 0.5f;
                player.ChangeHealth(-c.strength);
                yield return new WaitForSeconds(0.25f);

                c.GetComponent<BoardItem>().offset = Vector3.zero;
            }
            
        }

        foreach(Creature c in creatures){
            c.SetHealth(c.creatureType.health);
            c.SetStrength(c.creatureType.strength);
        }

        TurnManager.EndEnemyTurn();
    }

}
