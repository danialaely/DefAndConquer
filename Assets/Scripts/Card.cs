using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card
{
    public int cardId;
    public string cardName;
    public int cardAttack;
    public int cardHealth;
    public int cardEnergy;
    public int cardDefence;
    public Sprite cardImage;

    public Card() { }
    public Card(int id, string name, int attack, int health, int energy, Sprite cardImg, int defence)
    {
        cardId = id;
        cardName = name;
        cardAttack = attack;
        cardHealth = health;
        cardEnergy = energy;
        cardImage = cardImg;
        cardDefence = defence;
    }

}
