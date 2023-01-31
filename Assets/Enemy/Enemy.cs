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
        foreach(Creature c in creatures){
            c.SetHealth(c.creatureType.health);
            c.SetStrength(c.creatureType.strength);
        }

        StartCoroutine(TurnSequence());
    }

    IEnumerator TurnSequence()
    {
        foreach(Creature c in creatures)
        {
            c.GetComponent<BoardItem>().offset = Vector3.down * 0.5f;
            player.ChangeHealth(-c.strength);
            yield return new WaitForSeconds(0.25f);

            c.GetComponent<BoardItem>().offset = Vector3.zero;
        }

        TurnManager.EndEnemyTurn();
    }

}
