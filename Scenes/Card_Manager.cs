//using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
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
        Normal,
        Draw2,//You can only play other draw2's or a block card. 
        Draw4,//TO BE ADDED
        Skipped,//TO BE ADDED (MAY NOT BE NESSASSARY)
        GivingCard,//You can play any card, when played it is given to the next player
        Discard1Card,//You can play any card, when played it is sent to the deck instead of the discard pile (kinda a misnomer but whatever)
        Discard2Card,//TO BE ADDED
        CloneACard,//TO BE ADDED
        RerollACard,//TO BE ADDED

    }
    public bool actuallyStartGame = true;
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
        public string FlavorText;//flavor text
        public Sprite CardImage;//image to use
        public Card_Script.TagType[] Tags;//what tags this card has
        public Card_Script.AbilityType AbilityIndex;//what abilities does this card have?
        public CardData(string name, string description, Sprite cardImage, Card_Script.TagType[] tags, Card_Script.AbilityType abilityIndex)
        {
            Name = name;
            FlavorText = description;
            CardImage = cardImage;
            Tags = tags;
            AbilityIndex = abilityIndex;
        }
    };
    private Sprite loadSpr(string name)//makes defining cards quicker
    {
        return Resources.Load<Sprite>("Images/Card_Pictures/" + name);
    }
    //creates deck and gives starting cards
    void Start()
    {
        if (actuallyStartGame)
        {
            InitDeck();
            StartNewGame();
        }
        
    }


    public void updatePlayerCardPositions()//updates all card target positions, also sets their layers to be based on hand index
    {
        GameObject playerCard;
        for (int i = 0; i < PlayerCards.Count; i++)
        {
            playerCard = PlayerCards[i];
            playerCard.transform.rotation = new Quaternion(0, 0, 0, 0);
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
            if (playerCard.GetComponent<Card_Script>().IsFaceDown)
            {
                playerCard.transform.rotation = new Quaternion(0, 0, 0, 0);
            }    else
            {
                playerCard.transform.rotation = new Quaternion(0, 0, Mathf.PI, 0);
            }
                
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
            playerCard.transform.rotation = new Quaternion(0, 0, Mathf.PI / 2f, 0);
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
            playerCard.transform.rotation = new Quaternion(0, 0, Mathf.PI * 1.5f, 0);
            playerCard.GetComponent<Card_Script>().targetPosition = new Vector3(
                0 - 4.5f,
                i * 1.4f - Bot3Cards.Count / 2.0f * 1.4f,
                0
                );
            playerCard.GetComponent<Card_Script>().SetSortingLayer(i);
        }
    }
    //Plays the card, cardholder is the player who played it. 
    public void PlayCard(GameObject card, int cardHolder)
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
        
        card.GetComponent<Card_Script>().targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        card.GetComponent<Card_Script>().setIsFaceDown(false);
        card.transform.rotation = new Quaternion(0, 0, 0, 0);
        RemoveFromAllLists(card);
        updatePlayerCardPositions();
        //if we are on a giving turn, give this to next player and dont run any abilities
        if (CurrentTurnType == TurnType.GivingCard)
        {
            DrawCard(GetNextTurnNumber(), card);
            card.GetComponent<Card_Script>().setIsFaceDown(false);
            MoveToNextTurn(TurnType.Normal, 1);
            return;
        }
        //if this is a discard turn, discard and cancel any abilities
        else if (CurrentTurnType == TurnType.Discard1Card)
        {
            card.GetComponent<Card_Script>().Discard();
            MoveToNextTurn(TurnType.Normal, 1);
            return;
        }
        else
        {
            LastPlayedCard = card;
            Discard.Add(card);
        }
        //go to next turn if player/bot. also handle special
        if (cardHolder != -1)
        {
            Card_Script.AbilityType ability = card.GetComponent<Card_Script>().Ability;
            if (ability == Card_Script.AbilityType.Draw2)
            {
                MoveToNextTurn(TurnType.Draw2, 1);
            }
            else if (ability == Card_Script.AbilityType.Block)
            {
                MoveToNextTurn(TurnType.Normal, 1);
            }
            else if (ability == Card_Script.AbilityType.Give1Card)
            {
                MoveToNextTurn(TurnType.GivingCard, 0);
            }
            else if (ability == Card_Script.AbilityType.Discard1Card)
            {
                MoveToNextTurn(TurnType.Discard1Card, 0);
            }
            else if (ability == Card_Script.AbilityType.CoffeeBonusTurn)
            {
                MoveToNextTurn(TurnType.Normal, 0);
            }
            else if (ability == Card_Script.AbilityType.TakeAbilityCard)
            {
                if (GetRandomAbilityCard(GetNextTurnNumber(1)) != null) {
                    DrawCard(currentTurn, GetRandomAbilityCard(GetNextTurnNumber(1))).GetComponent<Card_Script>().setIsFaceDown(false);
                    MoveToNextTurn(TurnType.Discard1Card, 0);
                    
                } else
                {
                    MoveToNextTurn(TurnType.Normal, 1);
                }
                
            }
            else
            {
                MoveToNextTurn(TurnType.Normal, 1);
            }




        }
    }
    public GameObject GetRandomAbilityCard(int player)
    {
        List<GameObject> deck= PlayerCards;
        List<GameObject> abilityCards = new List<GameObject>();
        if (player == 0)
        {
            deck = PlayerCards;
        } else if (player == 1) 
        {
            deck = Bot1Cards;
        }
        else if (player == 2)
        {
            deck = Bot2Cards;
        }
        else if (player == 3)
        {
            deck = Bot3Cards;
        }
        for (int i = 0; i < deck.Count; i++) {
            if (deck[i].GetComponent<Card_Script>().Ability != Card_Script.AbilityType.None)
            {
                abilityCards.Add(deck[i]);
            }
        
        }
        Debug.Log(abilityCards.Count);
        if (abilityCards.Count == 0)
        {
            return null;
        }
        return abilityCards[UnityEngine.Random.Range(0, abilityCards.Count)];


    }
    //shuffles the deck
    public void ShuffleDeck()
    {
        GameObject card1;
        GameObject card2;
        int randCardIndex;
        for (int i = 0; i < Deck.Count; i++)
        {
            card1 = Deck[i];
            randCardIndex = UnityEngine.Random.Range(0, Deck.Count);
            card2 = Deck[randCardIndex];
            Deck[i] = card2;
            Deck[randCardIndex] = card1;
        }
    }
    public CardData[] getAllCards()
    {
        CardData[] AllTheFuckingCards =
{
            new CardData("Icky Garlic",
            "Hes actually the reason our universe exists. You may not like it, but this is the face of god. Bow before your creator.",
            loadSpr("Icky_Garlic"),
            new Card_Script.TagType[] {Card_Script.TagType.Gross,Card_Script.TagType.Earth},
            Card_Script.AbilityType.None),

            new CardData("Hot Garbage",
            "Litterally just me every sunday! minus the hot part. hahaha. also im crashing tf out bro i have so much work like fuck fuck fuck my life is so over how am i ever supposed to do everything i need to its all so done ahhh what do i do ahhhhh fuc fuc kfuc kfu ck.",
            loadSpr("hotgarbage"),
            new Card_Script.TagType[] {Card_Script.TagType.Gross,Card_Script.TagType.Fire},
            Card_Script.AbilityType.Discard1Card),

            new CardData("Spoiled milk",
            "Sadly, i was to nervious to recommend my idea for a gross water element....",
            loadSpr("Gomi_1"),
            new Card_Script.TagType[] {Card_Script.TagType.Gross,Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("The fart!",
            "You think youre the shit?? no dude ur the fart!!!!!!",
            loadSpr("Poop"),
            new Card_Script.TagType[] {Card_Script.TagType.Gross,Card_Script.TagType.Air},
            Card_Script.AbilityType.None),



            new CardData("Gorange the orage",
            "I got this as a gift 5 years ago. Ive been having a lot of weird dreams since....",
            loadSpr("Untouched_Grass"),
            new Card_Script.TagType[] {Card_Script.TagType.Real, Card_Script.TagType.Earth},
            Card_Script.AbilityType.None),

            new CardData("AUGEST 12 2036.",
            "THE HEAT DEATH OF THE UNIVERSE",
            loadSpr("Cat"),
            new Card_Script.TagType[] {Card_Script.TagType.Real, Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("Fishing rod",
            "I dont fucking know what to write here its a fucking fishing rod. fuck you, what do you want me to fucking say.",
            loadSpr("YOU"),
            new Card_Script.TagType[] {Card_Script.TagType.Real, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Card",
            "",
            loadSpr("Cards"),//TODO: GET IMAGE FOR THIS L8R
            new Card_Script.TagType[] {Card_Script.TagType.Real,Card_Script.TagType.Air},
            Card_Script.AbilityType.None),




            new CardData("Giving tree!",
            "Kindness is free. sprinkle that stuff everywhere! Your enemies are sure to be very happy with your lovely gift!",
            loadSpr("giving_tree"),
            new Card_Script.TagType[] {Card_Script.TagType.Cute,Card_Script.TagType.Earth},
            Card_Script.AbilityType.Give1Card),

            new CardData("Chubby Moth",
            "Pretty fly",
            loadSpr("Moth"),
            new Card_Script.TagType[] {Card_Script.TagType.Cute,Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("???",
            "Collect my 8 pages",
            loadSpr("Elf"),
            new Card_Script.TagType[] {Card_Script.TagType.Cute,Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Paper doll!",
            "It's in my head!",
            loadSpr("PaperDolls"),
            new Card_Script.TagType[] {Card_Script.TagType.Cute,Card_Script.TagType.Air},
            Card_Script.AbilityType.None),



            new CardData("Cursed pumpkin",
            "The fire obviously isnt real. You think a pumpkin would be able to haunt you and shit if it was on fucking fire?? dont be stupid. ",
            loadSpr("Cursed_Forest"),
            new Card_Script.TagType[] {Card_Script.TagType.Spooky,Card_Script.TagType.Earth},
            Card_Script.AbilityType.None),

            new CardData("Cursed candle!",
            "its really cute! Unfortunatly, if you are near it for too long, you go to hell before you die.",
            loadSpr("Spooky Candle"),
            new Card_Script.TagType[] {Card_Script.TagType.Spooky, Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("Guest",
            "He smells like bleach...",
            loadSpr("Guest"),
            new Card_Script.TagType[] {Card_Script.TagType.Spooky, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Ominous computer",
            "what the fuck is up with that thing? ;>_>",
            loadSpr("SpookyComputer"),
            new Card_Script.TagType[] {Card_Script.TagType.Spooky, Card_Script.TagType.Air},
            Card_Script.AbilityType.None),


            new CardData("Tanglekelp",
            "Hes so sneaky (WHAT DO I PUT HERE WHAT DO I SAY FUCK FUCK FUCK)",
            loadSpr("Cool ass tanglekelp"),
            new Card_Script.TagType[] {Card_Script.TagType.Sneaky,Card_Script.TagType.Earth},
            Card_Script.AbilityType.None),

            new CardData("Star man",
            "He still needs to learn how to noclip better",
            loadSpr("Starman"),
            new Card_Script.TagType[] {Card_Script.TagType.Sneaky, Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("Spy from TF2",
            "Yes, there are 2 sexes. The one I had with the effiel tower, and the one the effiel tower had with me.",
            loadSpr("Spy_tf2"),
            new Card_Script.TagType[] {Card_Script.TagType.Sneaky, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Glass!",
            "Killer of birds and embarrasser of dumb people (me)",
            loadSpr("Glass"),
            new Card_Script.TagType[] {Card_Script.TagType.Sneaky, Card_Script.TagType.Air},
            Card_Script.AbilityType.None),


            new CardData("Overgrown catpurrter!",
            "''Yeah i mean my depth purrseption is a lot worse now, but i think the flower has aura so it was a good tradeoff!''",
            loadSpr("digital_plant_1"),
            new Card_Script.TagType[] {Card_Script.TagType.Digital,Card_Script.TagType.Earth},
            Card_Script.AbilityType.None),

            new CardData("Firewall",
            "Yo this mother fucker always gettin in my way when i be downloading shit. like bro lay off and stop deleting my shit. god damn.",
            loadSpr("firewall_guy"),
            new Card_Script.TagType[] {Card_Script.TagType.Digital, Card_Script.TagType.Fire},
            Card_Script.AbilityType.Block),

            new CardData("Vicsine!",
            "Wants to bring you closer to others! ",
            loadSpr("Vicsine"),
            new Card_Script.TagType[] {Card_Script.TagType.Digital, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Blender cube",
            "Somebody fucking kill this thing already.",
            loadSpr("Blender_Cube"),
            new Card_Script.TagType[] {Card_Script.TagType.Digital, Card_Script.TagType.Air},
            Card_Script.AbilityType.None),


            new CardData("Chomper",
            "This thing always fucking sucks... still looks cool i guess",
            loadSpr("Chomper"),
            new Card_Script.TagType[] {Card_Script.TagType.Cool,Card_Script.TagType.Earth},
            Card_Script.AbilityType.None),

            new CardData("The sun",
            "''HEY GUYS ITS ME THE SUN. PLEASE STOP OVERLOOKING ME IM LIKE THE REASON YALL EXIST AND STUFF. ALSO SORRY ABOUT THE DROUTS AND STUFF MY BAD MY BAD...''",
            loadSpr("cool_sun"),
            new Card_Script.TagType[] {Card_Script.TagType.Cool, Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("Tony",
            "he owns a number factory",
            loadSpr("Tony"),
            new Card_Script.TagType[] {Card_Script.TagType.Cool, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Cool Bird",
            "HOLY SHIT IS THAT COOL BIRD?????!!!",
            loadSpr("Cool_Bird"),
            new Card_Script.TagType[] {Card_Script.TagType.Cool, Card_Script.TagType.Air},
            Card_Script.AbilityType.None),



            new CardData("Cuppa joe",
            "Coffee is the perfect way to start your day! unfortunatly, canabalism is illigal.",
            loadSpr("Cuppa_joe"),
            new Card_Script.TagType[] {Card_Script.TagType.Melancholic,Card_Script.TagType.Earth},
            Card_Script.AbilityType.CoffeeBonusTurn),

            new CardData("Kurble",
            "Another child star's life ruined with drugs....",
            loadSpr("Kurble"),
            new Card_Script.TagType[] {Card_Script.TagType.Melancholic, Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("._.",
            "I mean an alrightish rock i guess... ",
            loadSpr("PeriodUnderscorePeriod"),
            new Card_Script.TagType[] {Card_Script.TagType.Melancholic, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Dad's lost coin",
            "... I remeber this...",
            loadSpr("DadsLostCoin"),
            new Card_Script.TagType[] {Card_Script.TagType.Melancholic, Card_Script.TagType.Air},
            Card_Script.AbilityType.None),



            new CardData("Coffee Bean",
            "u know coffee spelt backwards is effco? what the fuck does that mean?",
            loadSpr("Bean"),
            new Card_Script.TagType[] {Card_Script.TagType.Magical,Card_Script.TagType.Earth},
            Card_Script.AbilityType.None),

            new CardData("This fucking cat whos name I forget",
            "It can do tricks and stuff",
            loadSpr("Cat_Magic"),
            new Card_Script.TagType[] {Card_Script.TagType.Magical, Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("Dark Evil Shapeshifting Tweaker",
            "LMAO i hope i forget to remove this name that would be funny af",
            loadSpr("Fishsune_Miku"),
            new Card_Script.TagType[] {Card_Script.TagType.Magical, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Bent spoon",
            "GET BENT NERD.",
            loadSpr("Spoonbender"),
            new Card_Script.TagType[] {Card_Script.TagType.Magical, Card_Script.TagType.Air},
            Card_Script.AbilityType.None),



            new CardData("Reverse giving tree",
            "Kindness may be free, but stealing gets you stuff. Do the math.",
            loadSpr("reverse_giving_tree"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil,Card_Script.TagType.Earth},
            Card_Script.AbilityType.TakeAbilityCard),

            new CardData("Green Demon",
            "Hes so fucking evil",
            loadSpr("Green_Demon"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil, Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("The Antichrist",
            "Hes so fucking evil",
            loadSpr("Antichrist"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Missing page",
            "Its cursed and shit and fucking evil and cursed and shit",
            loadSpr("MissingPage"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil, Card_Script.TagType.Air},
            Card_Script.AbilityType.None),



            new CardData("Bald flower",
            "perfect hospital gift!",
            loadSpr("Bald"),
            new Card_Script.TagType[] {Card_Script.TagType.Bald,Card_Script.TagType.Earth},
            Card_Script.AbilityType.None),

            new CardData("These fucking cats",
            "Lmao why the fuck do the look like this what the fuck",
            loadSpr("Cat_bald"),
            new Card_Script.TagType[] {Card_Script.TagType.Bald, Card_Script.TagType.Fire},
            Card_Script.AbilityType.None),

            new CardData("NL",
            "Okay, Okay!",
            loadSpr("OkayOkay"),
            new Card_Script.TagType[] {Card_Script.TagType.Bald, Card_Script.TagType.Water},
            Card_Script.AbilityType.None),

            new CardData("Shadless lamp",
            "No shade but i think you might be fucking stuipd. ",
            loadSpr("Bald_Lamp"),
            new Card_Script.TagType[] {Card_Script.TagType.Bald, Card_Script.TagType.Air},
            Card_Script.AbilityType.None),









            new CardData("Swarm of trained crow assassins",
            "HOLY SHIT (next player draws 2 cards and skips next turn)",
            loadSpr("crow_swarm"),
            new Card_Script.TagType[] {Card_Script.TagType.Devious, Card_Script.TagType.Fire},
            Card_Script.AbilityType.Draw2),

            new CardData("Evil Autism Weed",
            "The next player draws 2 cards and skips their turn.",
            loadSpr("Evil_autism_weed"),
            new Card_Script.TagType[] {Card_Script.TagType.Devious,Card_Script.TagType.Earth},
            Card_Script.AbilityType.Draw2),

            new CardData("Unus + Annus",
            "Yo why the fuck would they name that guy anus what the fuck. (next player draws 2 cards and skips next turn)",
            loadSpr("Unus_Annus"),
            new Card_Script.TagType[] {Card_Script.TagType.Devious,Card_Script.TagType.Water},
            Card_Script.AbilityType.Draw2),

            new CardData("Goathead",
            "THE GOAT (next player draws 2 and skips turn)",
            loadSpr("Goat_head"),
            new Card_Script.TagType[] {Card_Script.TagType.Devious,Card_Script.TagType.Air},
            Card_Script.AbilityType.Draw2),

            new CardData("Santa",
            "(next player draws 2 cards and skips next turn)",
            loadSpr("Santa"),
            new Card_Script.TagType[] {Card_Script.TagType.Jolly, Card_Script.TagType.Water},
            Card_Script.AbilityType.Draw2),

            new CardData("ME!!!",
            "FUCK YOU. The next player draws 2 cards and skips their turn.",
            loadSpr("Me"),
            new Card_Script.TagType[] {Card_Script.TagType.Jolly,Card_Script.TagType.Fire},
            Card_Script.AbilityType.Draw2),

            new CardData("Giving Tree",
            "Kindness is free, sprinkle that stuff everywhere! (next player draws 2 and skips turn).",
            loadSpr("givingTree"),
            new Card_Script.TagType[] {Card_Script.TagType.Jolly,Card_Script.TagType.Earth},
            Card_Script.AbilityType.Draw2),

            new CardData("Santa hat",
            "STOP LOOKING AT THE CAT. THIS CARD IS ABOUT THE HAT. NOT THE CAT. IGNORE THE CAT. (next player draws 2 and skips turn).",
            loadSpr("santa_hat"),
            new Card_Script.TagType[] {Card_Script.TagType.Jolly,Card_Script.TagType.Air},
            Card_Script.AbilityType.Draw2),
        };

        return AllTheFuckingCards;
    }
    //called once at the start, sets up the deck by creating a bunch of card objects using the array of cardDatas.
    public void InitDeck()
    {

        CardData[] AllTheFuckingCards = getAllCards();
        CardData[] TestCards =
        {

            new CardData("Reverse giving tree",
            "Kindness may be free, but stealing gets you stuff. Do the math.",
            loadSpr("reverse_giving_tree"),
            new Card_Script.TagType[] {Card_Script.TagType.Evil,Card_Script.TagType.Earth},
            Card_Script.AbilityType.TakeAbilityCard),
        };
        GameObject newCard;
        //create all the normal cards
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
        for (int i = 0; i < TestCards.Length; i++)
        {
            newCard = Instantiate(CardPrefab);
            Card_Script newCardScript = newCard.GetComponent<Card_Script>();
            newCardScript.manager = this;
            newCardScript.copyFromCardData(TestCards[i]);
            newCardScript.cardIndex = i + AllTheFuckingCards.Length;
            DrawCard(0, newCard);
            updatePlayerCardPositions();


        }
        ShuffleDeck();
    }
    //gets top card from deck (duh) if the deck less than 3 cards left, reshuffles the played cards into the deck
    public GameObject GetTopCard()
    {
        if (Deck.Count <= 3)
        {
            //go thru all but last played cards, add them to deck
            for (int i = 0; i < Discard.Count - 2; i++)
            {
                Discard[i].GetComponent<Card_Script>().SetSortingLayer(Discard[i].GetComponent<Card_Script>().cardIndex);
                Discard[i].GetComponent<Card_Script>().setIsFaceDown(true);
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
        return Deck[0];
    }
    //removes top card from deck (duh)
    public void RemoveTopCard()
    {
        Deck.RemoveAt(0);
    }
    //removes the top card from the deck and returns it. Also reshuffles discard into deck if deck length is < 1
    //gives cards to everyone and places inital card
    public void StartNewGame()
    {
        if (actuallyStartGame)
        {
        turnDirection = 1;
        for (int i = 0; i < PlayerCount; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                DrawCard(i, GetTopCard());
            }
        }
        PlayCard(GetTopCard(), -1);
        RemoveTopCard();
        }
    }
    //removes the card from any lists it in, sets player card to false, sets sorting layer to be card index
    public void RemoveFromAllLists(GameObject card)
    {
        card.GetComponent<Card_Script>().IsPlayerCard = false;
        card.GetComponent<Card_Script>().SetSortingLayer(card.GetComponent<Card_Script>().cardIndex);
        Deck.Remove(card);
        Discard.Remove(card);
        PlayerCards.Remove(card);
        Bot1Cards.Remove(card);
        Bot2Cards.Remove(card);
        Bot3Cards.Remove(card);
    }
    //Give the specified player a card, if card is null gives top from deck
    public GameObject DrawCard(int PlayerDrawing, GameObject DrawnCard)
    {
        RemoveFromAllLists(DrawnCard);
        if (PlayerDrawing == 0)
        {
            PlayerCards.Add(DrawnCard);
            DrawnCard.GetComponent<Card_Script>().setIsFaceDown(false);
            DrawnCard.GetComponent<Card_Script>().IsPlayerCard = true;
            updatePlayerCardPositions();
        }
        else if (PlayerDrawing == 1)
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
    public void PlayerDrawCards(int player)//called when drawing cards via deck. its a function cuz mouse controls and non-mouse controls draw cards diffrently
    {
        if (CurrentTurnType == TurnType.Draw2)
        {
            for (int i = 0; i < drawAmount; i++)
            {
                DrawCard(player, GetTopCard());
            }
            MoveToNextTurn(TurnType.Normal, 1);
        }
        else
        {
            DrawCard(player, GetTopCard());
            MoveToNextTurn(TurnType.Normal, 1);
        }
    }

    public void PlayerDrawCards(int player, GameObject card)//same as above, but used by mouse controls to make all the cards come from the one u dragged
    {
        if (CurrentTurnType == TurnType.Draw2)
        {
            for (int i = 0; i < drawAmount; i++)
            {
                DrawCard(player, GetTopCard()).transform.position = card.transform.position;
            }
            MoveToNextTurn(TurnType.Normal, 1);
        }
        else
        {
            DrawCard(player, GetTopCard());
            MoveToNextTurn(TurnType.Normal, 1);
        }
    }
    //handles if the player draws a card. Also does bot turns
    void Update()
    {
        if (actuallyStartGame)
        {
            TextMeshPro txt = drawCardText.GetComponent<TextMeshPro>();
            txt.text = "";
            if (currentTurn == 0)
            {
                if (CurrentTurnType == TurnType.Discard1Card)
                {
                    drawCardText.transform.position = new Vector3(0f, 1.8f, 0f);
                    txt.text = "Play any card to discard it!";
                }
                else if (CurrentTurnType == TurnType.GivingCard)
                {
                    drawCardText.transform.position = new Vector3(0f, 1.8f, 0f);
                    txt.text = "Play any card to give it to your opponent!";
                }
                else
                {
                    Vector3 topCor = Camera.main.WorldToScreenPoint(new Vector3(2 + 0.6f, 0 + 1.15f, 0.0f));
                    Vector3 botCor = Camera.main.WorldToScreenPoint(new Vector3(2 - 0.6f, 0 - 1.15f, 0.0f));

                    if ((Input.mousePosition.x < topCor.x && Input.mousePosition.x > botCor.x) && (Input.mousePosition.y < topCor.y && Input.mousePosition.y > botCor.y))
                    {
                        if (CurrentTurnType == TurnType.Normal)
                        {
                            drawCardText.transform.position = new Vector3(4.68f, 0.09f, 0f);
                            txt.text = "< Draw a Card";
                        }
                        if (CurrentTurnType == TurnType.Draw2)
                        {
                            drawCardText.transform.position = new Vector3(4.68f, 0.09f, 0f);
                            txt.text = "< Draw " + drawAmount + " Cards";
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (mouseControl)
                            {
                                Card_Script drawCard = GetTopCard().GetComponent<Card_Script>();
                                drawCard.SetSortingLayer(100);
                                drawCard.followingCursor = true;
                            }
                            else
                            {
                                PlayerDrawCards(0);
                            }
                        }
                    }
                }
            }
            else
            {
                botDelayTimer += 1 * Time.deltaTime;
                if (botDelayTimer > 1)
                {
                    MakeBotTurn(currentTurn);
                }
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
        PlayerDrawCards(botIndex);

    }
    //moves to next turn. currently ignores direction. add support for that if we add swap cards
    public void MoveToNextTurn(TurnType nextTurnType, int howManyTurns)
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
        currentTurn = (currentTurn + howManyTurns) % PlayerCount;
    }

    public int GetNextTurnNumber(int howManyTurns = 1)
    {
        return (currentTurn + howManyTurns) % PlayerCount;
    }

}
