using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TurnPhase
{
    Idle,
    Wait
}

public class TurnManager : MonoBehaviour
{
    static TurnManager instance;
    static Player player;
    static Enemy enemy;
    static TurnPhase turnPhase = TurnPhase.Idle;
    static MonoBehaviour activePlayer;
    static bool waiting = false;
    public static UnityEvent<TurnPhase> OnPhaseChanged = new UnityEvent<TurnPhase>();

    void Awake(){
        if (!instance) instance = this;
        else Destroy(this);

        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
    }

    public static bool MyTurn(MonoBehaviour monoBehaviour) => (monoBehaviour == activePlayer && turnPhase == TurnPhase.Idle);

    public static void StartPlayerTurn(){
        activePlayer = player;
        player.StartTurn();
    }

    public static void EndPlayerTurn(){
        StartEnemyTurn();
    }

    public static void StartEnemyTurn(){
        activePlayer = enemy;
        enemy.StartTurn();
    }

    public static void EndEnemyTurn(){
        StartPlayerTurn();
    }

    static void ChangePhase(TurnPhase newPhase)
    {
        if (newPhase == turnPhase) return;
        turnPhase = newPhase;
        OnPhaseChanged.Invoke(turnPhase);
    }

    public static void StartWait() => ChangePhase(TurnPhase.Wait);
    public static void StopWait() => ChangePhase(TurnPhase.Idle);
}
