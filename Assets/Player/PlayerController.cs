using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    Player player;
    BoardItem selectedCard;
    BoardItem targetCreature;
    GameObject target;
    Card_SO selectedCardType;

    public Board hand;
    public Board enemyBoard;

    #region Player Control

    private void Awake()
    {
        player = GetComponent<Player>();
        hand = player.board;
    }

    void Update()
    {
        if (!TurnManager.MyTurn(player)) return;

        if (Input.GetKeyDown(KeyCode.Return)) player.EndTurn();

        if (Input.GetMouseButtonDown(0)) SelectCard();
        if (Input.GetMouseButtonUp(0)) TryPlayCard();

        if (selectedCard){
            hand.DragTarget(selectedCard, MousePosition);

            if (selectedCardType.targetType == TargetType.Creature){
                targetCreature = enemyBoard.GetTarget(MousePosition);
                if (targetCreature) {
                    enemyBoard.SetCurrentTarget(targetCreature);
                    target = targetCreature.gameObject;
                }
            }
            else{
                target = player.gameObject;
            }
        }

        hand.SetCurrentTarget(hand.GetTarget(MousePosition));

        if (!selectedCard) return;
    }

    void SelectCard(){
        selectedCard = hand.GetTarget(MousePosition);
        if (selectedCard) selectedCardType = selectedCard.gameObject.GetComponent<Card>().cardType;
    }

    void TryPlayCard(){
        if (!selectedCard) return;
        if (!enemyBoard.InArea(MousePosition)) {selectedCard = null; return;}

        player.TryPlayCard(selectedCard.GetComponent<Card>(), target);

        selectedCard = null;
    }

    Vector3 MousePosition{
        get {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            return p;
        }
    }
    #endregion
}
