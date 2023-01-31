using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public Enemy enemy;
    public Board board;
    public GameObject cardPrefab;

    public List<Card_SO> cardsInDeck;
    Stack<Card_SO> deck;
    List<Card> hand;
    List<Card_SO> discard;

    public int health;
    public int mana;
    public TextMeshProUGUI displayHealth;
    public TextMeshProUGUI displayMana;

    private void Awake()
    {
        enemy = FindObjectOfType<Enemy>();

        hand = new List<Card>();
        discard = new List<Card_SO>();
    }

    private void Start()
    {
        ChangeHealth(0);
        ChangeMana(0);
        ShuffleDeck();
        DrawCard();
        DrawCard();
        DrawCard();
        TurnManager.StartPlayerTurn();
    }

    public void ChangeHealth(int amount){
        health += amount;
        displayHealth.text = health.ToString();
    }

    public void SetMana(int amount)
    {
        mana = amount;
        displayMana.text = mana.ToString();
    }

    public void ChangeMana(int amount){
        mana += amount;
        displayMana.text = mana.ToString();
    }

    public void ShuffleDeck(){
        deck = new Stack<Card_SO>();
        while(cardsInDeck.Count > 0){
            int index = Random.Range(0, cardsInDeck.Count);
            deck.Push(cardsInDeck[index]);
            cardsInDeck.RemoveAt(index);
        }
    }

    public void DrawCard(){
        if (deck.Count <= 0) return;

        Card_SO cardType = deck.Pop();
        GameObject newCard = board.NewBoardItem(cardPrefab);
        Card card = newCard.GetComponent<Card>();

        card.LoadCard_SO(cardType);
        hand.Add(card);
    }

    public bool TryPlayCard(Card card, GameObject target) {
        if (mana < card.cardType.manaCost) return false;
        ChangeMana(-card.cardType.manaCost);
        DiscardCard(card);

        StartCoroutine(ResolveCardEffects(card.cardType, target));

        return true;
    }

    public void DiscardCard(Card card){
        board.RemoveBoardItem(card.GetComponent<BoardItem>());
        discard.Add(card.cardType);   
    }

    public void EndTurn(){
        TurnManager.EndPlayerTurn();
    }

    public void StartTurn()
    {
        DrawCard();
        SetMana(3);
    }

    IEnumerator ResolveCardEffects(Card_SO cardType, GameObject target){
        TurnManager.StartWait();

        Creature targetCreature = target.GetComponent<Creature>();

        for(int i = 0; i < cardType.OnPlayEffects.Count; i++){

            cardType.OnPlayEffects[i].ActivateEffect(target);

            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(0.25f);

        if (targetCreature && targetCreature.health <= 0)
        {
            for(int i = 0; i < cardType.OnKillEffects.Count; i++){
                cardType.OnKillEffects[i].ActivateEffect(target);

                yield return new WaitForSeconds(0.25f);
            }
        }

        TurnManager.StopWait();
    }

}
