//using NUnit.Framework.Internal;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Editor;
using UnityEngine.SocialPlatforms;


public class Card_Manager : MonoBehaviour
{
    public GameObject GameManager;
    public List<GameObject> Deck;
    public List<GameObject> Discard;
    public GameObject LastPlayedCard;//the top card in dicard
    //player cards
    public List<GameObject> PlayerCards;
    public List<GameObject> Bot1Cards;
    public List<GameObject> Bot2Cards;
    public List<GameObject> Bot3Cards;
    public enum TurnType
    {
        Normal, Draw2, Draw4, Skipped
    }
    public int drawAmount;//used to keep track of stacked +2's
    public TurnType CurrentTurnType;
    public GameObject drawCardText;//text that says "< Draw Card" when hovering over the deck on ur turn
    public float botDelayTimer;
    public int PlayerCount;//max # of players. 2 is good. 4 is rlly cramped lmao..
    public int currentTurn;
    public int turnDirection;
    public bool mouseControl;
    //public Randomizer Rand = new Randomizer();
    public GameObject CardPrefab;
    //class to store card data.
    public class CardData
    {
        public string Name;//name of card
        public string Description;//flavor text
        public Sprite CardImage;//image to use
        public Card_Script.TagType[] Tags;//what tags this card has
        public Card_Script.AbilityType AbilityIndex;//what abilities does this card have?
        public CardData(string name, string description, Sprite cardImage, Card_Script.TagType[] tags, Card_Script.AbilityType abilityIndex)
        {
            Name = name;
            Description = description;
            CardImage = cardImage;
            Tags = tags;
            AbilityIndex = abilityIndex;
        }
    };
    private Sprite loadSpr(string name)//makes defining cards quicker
    {
        return Resources.Load<Sprite>("Images/" + name);
    }
    //creates deck and gives starting cards
    void Start()
    {
        InitDeck();
        //StartNewGame();
    }

    public void updatePlayerCardPositions()
    {
        GameObject playerCard;
        for (int i = 0; i < PlayerCards.Count; i++)
        {
            playerCard = PlayerCards[i];

            playerCard.GetComponent<Card_Script>().targetPosition = new Vector3(
                i * 1.6f - PlayerCards.Count / 2.0f * 1.6f,
                0 - 3.5f,
                0
                );
            playerCard.GetComponent<Card_Script>().SetSortingLayer(i);

        }

        for (int i = 0; i < Bot1Cards.Count; i++)
        {
            
            playerCard = Bot1Cards[i];
            playerCard.transform.rotation = new Quaternion(0, 0,Mathf.PI, 0);
            playerCard.GetComponent<Card_Script>().targetPosition = new Vector3(
                i * 1.6f - Bot1Cards.Count / 2.0f * 1.6f,
                0 + 3.5f,
                0
                );
            playerCard.GetComponent<Card_Script>().SetSortingLayer(i);
        }
        for (int i = 0; i < Bot2Cards.Count; i++)
        {
            playerCard = Bot2Cards[i];
            playerCard.transform.rotation = new Quaternion(0, 0, Mathf.PI/2f, 0);
            playerCard.GetComponent<Card_Script>().targetPosition = new Vector3(
                0 + 4.5f,
                i * 1.4f - Bot2Cards.Count / 2.0f * 1.4f,
                0
                );
            playerCard.GetComponent<Card_Script>().SetSortingLayer(i);
        }
        for (int i = 0; i < Bot3Cards.Count; i++)
        {
            playerCard = Bot3Cards[i];
            playerCard.transform.rotation = new Quaternion(0, 0, Mathf.PI*1.5f, 0);
            playerCard.GetComponent<Card_Script>().targetPosition = new Vector3(
                0 - 4.5f,
                i * 1.4f - Bot3Cards.Count / 2.0f * 1.4f,
                0
                );
            playerCard.GetComponent<Card_Script>().SetSortingLayer(i);
        }
    }
    public void PlayCard(GameObject card,int cardHolder)
    {
        
        //move the 2nd discarded card further so we dont get a weird overlap when moving the current one down
        if (Discard.Count > 1)
        {
            GameObject card2 = Discard[Discard.Count - 2];
            if (card2.GetComponent<Card_Script>() != null)
            {
                card2.GetComponent<Card_Script>().SetSortingLayer(-10);
            }
        }
        //move the current card wayy down
        if (LastPlayedCard != null)
        {
            if (LastPlayedCard.GetComponent<Card_Script>() != null)
            {
                LastPlayedCard.GetComponent<Card_Script>().SetSortingLayer(-9);
            }
        }
        
        //move the card to discard
        LastPlayedCard = card;
        card.GetComponent<Card_Script>().targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        card.GetComponent<Card_Script>().setIsFaceDown(false);
        card.transform.rotation = new Quaternion(0, 0, 0, 0);
        if (cardHolder == 1)
        {
            Bot1Cards.Remove(card);
        }
        else if (cardHolder == 2)
        {
            Bot2Cards.Remove(card);
        }
        else if (cardHolder == 3)
        {
            Bot3Cards.Remove(card);
        }
        if (card.GetComponent<Card_Script>().IsPlayerCard)
        {
            PlayerCards.Remove(card);
            card.GetComponent<Card_Script>().IsPlayerCard = false;
        }
        Discard.Add(card);
        updatePlayerCardPositions();

        //go to next turn if player/bot. also handle special
        if (cardHolder != -1 )
        {
            if (card.GetComponent<Card_Script>().Ability == Card_Script.AbilityType.Draw2)
            {
                MoveToNextTurn(TurnType.Draw2);
            }else
            {
                MoveToNextTurn(TurnType.Normal);
            }

                


        }
    }
    public void ShuffleDeck()
    {
        GameObject card1;
        GameObject card2;
        int randCardIndex;
        for (int i = 0; i < Deck.Count; i++)
        {
            card1 = Deck[i];
            randCardIndex = Random.Range(0, Deck.Count - 1);
            card2 = Deck[randCardIndex];

            Deck[i] = card2;
            Deck[randCardIndex] = card1;
        }
    }
    public void InitDeck()
    {
        CardData[] AllTheFuckingCards =
        {
            new CardData("Icky Garlic",
            "GROSS!!! EW EW BAD!!! I AM GOING TO MOVE TO THE OTHER LANE!!!",
            loadSpr("Garlic"),
            new Card_Script.TagType[] {Card_Script.TagType.Gross,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("Dexter",
            "''Oh my god im dexter im the smartest mother fucker on the planet. Fuck jimmy neutron.''",
            loadSpr("Dexter"),
            new Card_Script.TagType[] {Card_Script.TagType.Gross,Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("Gomi!",
            "All rounder type baddie, can swoop in an save u or raize hellz!",
            loadSpr("Gomi_1"),
            new Card_Script.TagType[] {Card_Script.TagType.Gross,Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("The poop",
            "Plop!",
            loadSpr("Poop"),
            new Card_Script.TagType[] {Card_Script.TagType.Gross,Card_Script.TagType.Object},
            Card_Script.AbilityType.None),



            new CardData("Untouched grass",
            "its scary out there....",
            loadSpr("Untouched_Grass"),
            new Card_Script.TagType[] {Card_Script.TagType.Real, Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("Cat",
            "silly creature",
            loadSpr("Cat"),
            new Card_Script.TagType[] {Card_Script.TagType.Real, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("You.",
            "",
            loadSpr("YOU"),
            new Card_Script.TagType[] {Card_Script.TagType.Real, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Card",
            "",
            loadSpr("Cards"),//TODO: GET IMAGE FOR THIS L8R
            new Card_Script.TagType[] {Card_Script.TagType.Real,Card_Script.TagType.Object},
            Card_Script.AbilityType.None),




            new CardData("Chompy!",
            "(She bites)",
            loadSpr("Chompy"),
            new Card_Script.TagType[] {Card_Script.TagType.Cute,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("Chubby Moth",
            "Pretty fly",
            loadSpr("Moth"),
            new Card_Script.TagType[] {Card_Script.TagType.Cute,Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("???",
            "Collect my 8 pages",
            loadSpr("Elf"),
            new Card_Script.TagType[] {Card_Script.TagType.Cute,Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Paper doll!",
            "It's in my head!",
            loadSpr("PaperDolls"),
            new Card_Script.TagType[] {Card_Script.TagType.Cute,Card_Script.TagType.Object},
            Card_Script.AbilityType.None),



            new CardData("!!!",
            "its compleatly leafless....",
            loadSpr("Cursed_Forest"),
            new Card_Script.TagType[] {Card_Script.TagType.Spooky,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("Cursed Moth",
            "Its kinda cute...",
            loadSpr("Moth_Spooky"),
            new Card_Script.TagType[] {Card_Script.TagType.Spooky, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("Guest",
            "He smells like bleach...",
            loadSpr("Guest"),
            new Card_Script.TagType[] {Card_Script.TagType.Spooky, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Ominous computer",
            "what the fuck is up with that thing? ;>_>",
            loadSpr("SpookyComputer"),
            new Card_Script.TagType[] {Card_Script.TagType.Spooky, Card_Script.TagType.Object},
            Card_Script.AbilityType.None),


            new CardData("Tanglekelp",
            "Hes so sneaky (WHAT DO I PUT HERE WHAT DO I SAY FUCK FUCK FUCK)",
            loadSpr("Cool ass tanglekelp"),
            new Card_Script.TagType[] {Card_Script.TagType.Sneaky,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("Star man",
            "He still needs to learn how to noclip better",
            loadSpr("Starman"),
            new Card_Script.TagType[] {Card_Script.TagType.Sneaky, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("Spy from TF2",
            "Yes, there are 2 sexes. The one I had with the effiel tower, and the one the effiel tower had with me.",
            loadSpr("Spy_tf2"),
            new Card_Script.TagType[] {Card_Script.TagType.Sneaky, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Glass!",
            "Killer of birds and embarrasser of dumb people (me)",
            loadSpr("Glass"),
            new Card_Script.TagType[] {Card_Script.TagType.Sneaky, Card_Script.TagType.Object},
            Card_Script.AbilityType.None),


            new CardData("The infinut",
            "He is eternal. Pretty good plantfood 2!",
            loadSpr("Infinut"),
            new Card_Script.TagType[] {Card_Script.TagType.Digital,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("9331 Y@#R %K!N",
            "I think this is a virus or something?",
            loadSpr("PEELYOURSKIN"),
            new Card_Script.TagType[] {Card_Script.TagType.Digital, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("Vicsine!",
            "Wants to bring you closer to others! ",
            loadSpr("Vicsine"),
            new Card_Script.TagType[] {Card_Script.TagType.Digital, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Blender cube",
            "Somebody fucking kill this thing already.",
            loadSpr("Blender_Cube"),
            new Card_Script.TagType[] {Card_Script.TagType.Digital, Card_Script.TagType.Object},
            Card_Script.AbilityType.None),


            new CardData("Chomper",
            "This thing always fucking sucks... still looks cool i guess",
            loadSpr("Chomper"),
            new Card_Script.TagType[] {Card_Script.TagType.Cool,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("Blue Egg!",
            "Bigger. Brighter. Bolder. Older.",
            loadSpr("Egg2"),
            new Card_Script.TagType[] {Card_Script.TagType.Cool, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("Tony",
            "he owns a number factory",
            loadSpr("Tony"),
            new Card_Script.TagType[] {Card_Script.TagType.Cool, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Bowling floor",
            "I think it looks pretty cool! Reminds me i suck ass at bowling tho so that sucks.",
            loadSpr("Bowling"),
            new Card_Script.TagType[] {Card_Script.TagType.Cool, Card_Script.TagType.Object},
            Card_Script.AbilityType.None),



            new CardData("Lilypad",
            "People always be pushing this mf around. im tired of it!!!",
            loadSpr("Lily"),
            new Card_Script.TagType[] {Card_Script.TagType.Melancholic,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("Kurble",
            "Another child star's life ruined with drugs....",
            loadSpr("Kurble"),
            new Card_Script.TagType[] {Card_Script.TagType.Melancholic, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("._.",
            "I mean an alrightish rock i guess... ",
            loadSpr("PeriodUnderscorePeriod"),
            new Card_Script.TagType[] {Card_Script.TagType.Melancholic, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Dad's lost coin",
            "... I remeber this...",
            loadSpr("DadsLostCoin"),
            new Card_Script.TagType[] {Card_Script.TagType.Melancholic, Card_Script.TagType.Object},
            Card_Script.AbilityType.None),



            new CardData("Coffee Bean",
            "u know coffee spelt backwards is effco? what the fuck does that mean?",
            loadSpr("Bean"),
            new Card_Script.TagType[] {Card_Script.TagType.Magical,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("This fucking cat whos name I forget",
            "It can do tricks and stuff",
            loadSpr("Cat_Magic"),
            new Card_Script.TagType[] {Card_Script.TagType.Magical, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("Dark Evil Shapeshifting Tweaker",
            "Hes at the gas station rn",
            loadSpr("DarkEvilShapeshiftingTweaker"),
            new Card_Script.TagType[] {Card_Script.TagType.Magical, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Bent spoon",
            "GET BENT NERD.",
            loadSpr("Spoonbender"),
            new Card_Script.TagType[] {Card_Script.TagType.Magical, Card_Script.TagType.Object},
            Card_Script.AbilityType.None),



            new CardData("Happy Tomato",
            "Hes so fucking evil",
            loadSpr("Tomato"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("Green Demon",
            "Hes so fucking evil",
            loadSpr("Green_Demon"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("The Antichrist",
            "Hes so fucking evil",
            loadSpr("Antichrist"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Missing page",
            "Its cursed and shit and fucking evil and cursed and shit",
            loadSpr("MissingPage"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil, Card_Script.TagType.Object},
            Card_Script.AbilityType.None),



            new CardData("Bald flower",
            "perfect hospital gift!",
            loadSpr("Bald"),
            new Card_Script.TagType[] {Card_Script.TagType.Bald,Card_Script.TagType.Plant},
            Card_Script.AbilityType.None),

            new CardData("These fucking cats",
            "Lmao why the fuck do the look like this what the fuck",
            loadSpr("Cat_bald"),
            new Card_Script.TagType[] {Card_Script.TagType.Bald, Card_Script.TagType.Creature},
            Card_Script.AbilityType.None),

            new CardData("NL",
            "Okay, Okay!",
            loadSpr("OkayOkay"),
            new Card_Script.TagType[] {Card_Script.TagType.Bald, Card_Script.TagType.Person},
            Card_Script.AbilityType.None),

            new CardData("Shadless lamp",
            "No shade but i think you might be fucking stuipd. ",
            loadSpr("Bald_Lamp"),
            new Card_Script.TagType[] {Card_Script.TagType.Bald, Card_Script.TagType.Object},
            Card_Script.AbilityType.None),
        };

        CardData[] AllTheFuckingSpecialCards =
        {

            new CardData("Swarm of trained crow assassins",
            "HOLY SHIT (next player draws 2 cards and skips next turn)",
            loadSpr("crow_swarm"),
            new Card_Script.TagType[] {Card_Script.TagType.Devious, Card_Script.TagType.Creature},
            Card_Script.AbilityType.Draw2),

            new CardData("Evil Autism Weed",
            "The next player draws 2 cards and skips their turn.",
            loadSpr("Evil_autism_weed"),
            new Card_Script.TagType[] {Card_Script.TagType.Devious,Card_Script.TagType.Plant},
            Card_Script.AbilityType.Draw2),

            new CardData("Unus + Annus",
            "Yo why the fuck would they name that guy anus what the fuck. (next player draws 2 cards and skips next turn)",
            loadSpr("Unus_Annus"),
            new Card_Script.TagType[] {Card_Script.TagType.Devious,Card_Script.TagType.Person},
            Card_Script.AbilityType.Draw2),

            new CardData("Goathead",
            "THE GOAT (next player draws 2 and skips turn)",
            loadSpr("Goat_head"),
            new Card_Script.TagType[] {Card_Script.TagType.Devious,Card_Script.TagType.Object},
            Card_Script.AbilityType.Draw2),




            new CardData("Santa",
            "(next player draws 2 cards and skips next turn)",
            loadSpr("Santa"),
            new Card_Script.TagType[] {Card_Script.TagType.Jolly, Card_Script.TagType.Person},
            Card_Script.AbilityType.Draw2),

            new CardData("ME!!!",
            "FUCK YOU. The next player draws 2 cards and skips their turn.",
            loadSpr("Me"),
            new Card_Script.TagType[] {Card_Script.TagType.Jolly,Card_Script.TagType.Creature},
            Card_Script.AbilityType.Draw2),

            new CardData("Giving Tree",
            "Kindness is free, sprinkle that stuff everywhere! (next player draws 2 and skips turn).",
            loadSpr("givingTree"),
            new Card_Script.TagType[] {Card_Script.TagType.Jolly,Card_Script.TagType.Plant},
            Card_Script.AbilityType.Draw2),

            new CardData("Santa hat",
            "STOP LOOKING AT THE CAT. THIS CARD IS ABOUT THE HAT. NOT THE CAT. IGNORE THE CAT. (next player draws 2 and skips turn).",
            loadSpr("santa_hat"),
            new Card_Script.TagType[] {Card_Script.TagType.Jolly,Card_Script.TagType.Object},
            Card_Script.AbilityType.Draw2),


        };
        GameObject newCard;
        for (int i = 0; i < AllTheFuckingCards.Length; i++)
        {
            newCard = Instantiate(CardPrefab);
            Card_Script newCardScript = newCard.GetComponent<Card_Script>();
            newCardScript.manager = this;
            newCardScript.setIsFaceDown(true);
            newCardScript.copyFromCardData(AllTheFuckingCards[i]);
            newCardScript.cardIndex = i;
            Deck.Add(newCard);

        }
        for (int i = 0; i < AllTheFuckingSpecialCards.Length; i++)
        {
            newCard = Instantiate(CardPrefab);
            Card_Script newCardScript = newCard.GetComponent<Card_Script>();
            newCardScript.manager = this;
            newCardScript.setIsFaceDown(true);
            newCardScript.copyFromCardData(AllTheFuckingSpecialCards[i]);
            newCardScript.cardIndex = i;
            Deck.Add(newCard);

        }
        ShuffleDeck();
    }
    //gets top card from deck (duh)
    public GameObject GetTopCard()
    {
        return Deck[0];
    }
    //removes top card from deck (duh)
    public void RemoveTopCard()
    {
        Deck.RemoveAt(0);
    }
    //removes the top card from the deck and returns it. Also reshuffles discard into deck if deck length is < 1
    public GameObject DrawTopCard()
    {   
        //incase we run out of cards in the deck
        if (Deck.Count <= 1)
        {
            //go thru all but last discarded cards, add them to deck
            for (int i=0;i<Discard.Count-2;i++)
            {
                Discard[i].GetComponent<Card_Script>().SetSortingLayer(Discard[i].GetComponent<Card_Script>().cardIndex);
                Discard[i].GetComponent < Card_Script >().setIsFaceDown(true);
                Discard[i].GetComponent<Card_Script>().targetPosition = new Vector3(2, 0, 0);
                Deck.Add(Discard[i]);
            }
            //then remove them from discard.
            int l = Discard.Count - 2;
            for (int i = 0; i < l; i++)
            {
                Discard.RemoveAt(0);
            }

            ShuffleDeck();
        }
        
        GameObject topCard;
        topCard = GetTopCard();
        RemoveTopCard();
        return topCard;
    }
    //gives a card to the player/bot you specifiy. Also updates card positions for everyone
    public GameObject DrawNewCard(int PlayerDrawing)
    {
        GameObject DrawnCard = DrawTopCard();
        if (PlayerDrawing == 0)
        {
            PlayerCards.Add(DrawnCard);
            DrawnCard.GetComponent<Card_Script>().setIsFaceDown(false);
            DrawnCard.GetComponent<Card_Script>().IsPlayerCard = true;
            updatePlayerCardPositions();
        }
        else if(PlayerDrawing == 1)
        {
            DrawnCard.GetComponent<Card_Script>().setIsFaceDown(true);
            Bot1Cards.Add(DrawnCard);
            updatePlayerCardPositions();
        }
        else if (PlayerDrawing == 2)
        {
            DrawnCard.GetComponent<Card_Script>().setIsFaceDown(true);
            Bot2Cards.Add(DrawnCard);
            updatePlayerCardPositions();
        }
        else if (PlayerDrawing == 3)
        {
            DrawnCard.GetComponent<Card_Script>().setIsFaceDown(true);
            Bot3Cards.Add(DrawnCard);
            updatePlayerCardPositions();
        }
        return DrawnCard;
    }
    //gives cards to everyone and places inital card
    public void StartNewGame()
    {
        turnDirection = 1;
        for (int i = 0; i < PlayerCount; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                DrawNewCard(i);
            }
        }
        PlayCard(GetTopCard(),-1);
        RemoveTopCard();
    }

    //handles if the player draws a card. Also does bot turns
    void Update()
    {
        TextMeshPro txt = drawCardText.GetComponent<TextMeshPro>();
        txt.text = "";
        if (currentTurn == 0)
        {
            Vector3 topCor = Camera.main.WorldToScreenPoint(new Vector3(2 + 0.6f, 0 + 1.15f, 0.0f));
            Vector3 botCor = Camera.main.WorldToScreenPoint(new Vector3(2 - 0.6f, 0 - 1.15f, 0.0f));

            if ((Input.mousePosition.x < topCor.x && Input.mousePosition.x > botCor.x) && (Input.mousePosition.y < topCor.y && Input.mousePosition.y > botCor.y))
            {
                txt.text = "< Draw a Card";
                if (CurrentTurnType == TurnType.Draw2)
                {
                    txt.text = "< Draw " + drawAmount + " Cards";
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (mouseControl)
                    {
                        Card_Script drawCard = GetTopCard().GetComponent<Card_Script>();
                        drawCard.SetSortingLayer(100);
                        drawCard.followingCursor = true;
                    } else
                    {
                        if (CurrentTurnType == TurnType.Draw2)
                        {
                            for (int i=0; i<drawAmount;i++)
                            {
                                DrawNewCard(0);
                            }
                            MoveToNextTurn(TurnType.Normal);
                        } else
                        {
                            DrawNewCard(0);
                            MoveToNextTurn(TurnType.Normal);
                        }
                            
                    }
                        

                }
            }
        } else
        {
            botDelayTimer += 1*Time.deltaTime;
            if (botDelayTimer > 1)
            {
                MakeBotTurn(currentTurn);
            }
        }
    }
    //allows the bot at this index to play/draw a card. will need more support for when to play specials.
    //along with support for cards that require a choice
    public void MakeBotTurn(int botIndex)
    {
        List<GameObject> DeckToUse;
        if (botIndex == 1)
        {
            DeckToUse = Bot1Cards;
        }
        else if (botIndex == 2)
        {
            DeckToUse = Bot2Cards;
        } 
        else if (botIndex == 3)
        {
            DeckToUse = Bot3Cards;
        }
        else
        {
            //Debug.Log("Something went wrong with bot turns bozo, input should only be 1-3. u gave me somefin else!! WHAT DO I DOOOOOOO");
            return;
        }
        Card_Script currentCardScript;
        for (int i = 0; i < DeckToUse.Count; i++)
        {
            currentCardScript = DeckToUse[i].GetComponent<Card_Script>();
            if (currentCardScript.CanBePlayed())
            {
                PlayCard(DeckToUse[i], botIndex);
                return;
            }
        }
        if (CurrentTurnType == TurnType.Normal)
        {
            DrawNewCard(botIndex);
        } else if (CurrentTurnType == TurnType.Draw2)
        {
            for (int i = 0; i < drawAmount; i++)
            {
                DrawNewCard(botIndex);
            }
        }
            MoveToNextTurn(TurnType.Normal);

    }
    //moves to next turn. currently ignores direction. add support for that if we add swap cards
    public void MoveToNextTurn(TurnType nextTurnType)
    {
        botDelayTimer = 0;
        if (nextTurnType == TurnType.Normal)
        {
            drawAmount = 0;//reset stacked +2's
        }
        if (nextTurnType == TurnType.Draw2)
        {
            drawAmount += 2;//add 2 to amount of card 2 draw 
        }

        CurrentTurnType = nextTurnType;
        currentTurn = (currentTurn + 1) % PlayerCount;
    }
}
