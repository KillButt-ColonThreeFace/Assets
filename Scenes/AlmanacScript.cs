using System.Collections.Generic;
using UnityEngine;

public class AlmanacScript : MonoBehaviour

{
    public Transform startPos;
    public GameObject CardPrefab;
    public List<GameObject> Deck;
    public int speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initCards();
        
    }

    // Update is called once per frame
    void Update()


    {
        for (int i = 0; i<Deck.Count; i++)
        {
            Deck[i].GetComponent<Card_Script>().setPos(Deck[i].GetComponent<Card_Script>().getPos().x, Deck[i].GetComponent<Card_Script>().getPos().y + (Input.GetAxis("Mouse ScrollWheel")*speed));

        }
    }
    public GameObject manager;
    private Sprite loadSpr(string name)//makes defining cards quicker
    {
        return Resources.Load<Sprite>("Images/" + name);
    }
    private void initCards()
    {
        Card_Manager.CardData[] AllTheFuckingCards = manager.GetComponent<Card_Manager>().getAllCards();
        GameObject newCard;
        for (int i = 0; i < AllTheFuckingCards.Length; i++)
        {
            newCard = Instantiate(CardPrefab);
            Card_Script newCardScript = newCard.GetComponent<Card_Script>();
            newCardScript.almanac = this;
            newCardScript.Alm = true;
            newCardScript.setIsFaceDown(true);
            newCardScript.copyFromCardData(AllTheFuckingCards[i]);
            newCardScript.cardIndex = i;
            Deck.Add(newCard);

        }
        for (int i = 0; i< Deck.Count; i++)
        {
            Deck[i].GetComponent<Card_Script>().setIsFaceDown(false);
            Deck[i].GetComponent<Card_Script>().setPos(startPos.position.x+(2*(i%7)), startPos.position.y-(3*(i/7)));


        }
                



    }

}
