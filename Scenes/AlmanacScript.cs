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
    private void initCards()
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
            newCardScript.almanac = this;
            newCardScript.Alm = true;
            newCardScript.setIsFaceDown(true);
            newCardScript.copyFromCardData(AllTheFuckingCards[i]);
            newCardScript.cardIndex = i;
            print("hi");
            Deck.Add(newCard);

        }
        for (int i = 0; i < AllTheFuckingSpecialCards.Length; i++)
        {
            newCard = Instantiate(CardPrefab);
            Card_Script newCardScript = newCard.GetComponent<Card_Script>();
            newCardScript.almanac = this;
            newCardScript.Alm = true;
            newCardScript.setIsFaceDown(true);
            newCardScript.copyFromCardData(AllTheFuckingSpecialCards[i]);
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
