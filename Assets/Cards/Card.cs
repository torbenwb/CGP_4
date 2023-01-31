using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public Card_SO cardType;
    public Image cardImage;
    public new TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public TextMeshProUGUI manaCost;

    public void LoadCard_SO(Card_SO cardType){
        this.cardType = cardType;
        cardImage.sprite = cardType.image;
        name.text = cardType.name;
        description.text = cardType.description;
        manaCost.text = cardType.manaCost.ToString();
    }
}
