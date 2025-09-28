using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;

//using Unity.VisualScripting;
//using UnityEditor;
using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
public class Card_Script : MonoBehaviour
{
    public AlmanacScript almanac;
    public bool Alm = false;
    //enum for the tag types
    public enum TagType
    {
        Earth, Fire, Water, Air, Wild,//"main" tags
        Gross, Real, Cute, Spooky, Sneaky, Digital, Cool, Melancholic, Magical, Evil, Bald,//"seconday" tags

        Devious, Jolly,//"special" tags (used as 1-offs for some special cards)

        
    }
    public float hoverOverlayAlpha = 0.0f;
    //enum for the abilities
    public enum AbilityType
    {
        None,
        Skip, 
        Give1Card, 
        KillTheOtherPlayer, 
        Draw2, 
        Draw4, 
        Reverse, 
        Block, 
        Discard1Card,
        CoffeeBonusTurn,
        TakeAbilityCard,
    }
    private static string[] TextForAbilities = new string[] {
        "No ability!",
        "Skip the next players turn!",
        "When played, choose a card from your hand to give to another player!",
        "Kill the other player.",
        "Next player draws 2, unless they play another card with this ability. In which case, the player after that will draw four (unless they also play a card with this ability).",
        "AHH",
        "AHHHH",
        "Can be played when you are effected by an ability that makes you draw cards. If played, will cancel these effects",
        "When played, choose a card from your hand to discard",
        "Playing this card doesn't end your turn!",
        "If your op has a card with an ability, take it and then choose a card to discard. otherwise, does nothing.",
    };

    //list of the tag names (used for auto-generating the tag text)
    private static bool[] TagIsSecondary = new bool[] {
        false,false,false,false,false,
        true,true,true,true,true,true,true,true,true,true,true,

        true,true,
        };
    private static string[] TagNames = new string[] {
        "EARTH", "FIRE", "WATER", "AIR","WILD",
        "gross", "real", "cute", "spooky", "sneaky", "digital", "cool", "melancholic", "magical", "evil","bald",

        "Devious","Jolly!",
    };
    public Card_Manager manager;//reference to card manager object
    public List<TagType> Tags;//what tags the card has

    public GameObject BorderColor;
    public GameObject BorderOutline;
    public GameObject TagImage;
    public GameObject TagImageOutline;
    public GameObject HoverOverlay;
    public GameObject CardBack;

    public GameObject FlavorText;//ref to the desctiption text object
    public GameObject AbilityText;//ref to ability object

    public GameObject NameText;//ref to the name text object
    public GameObject NameText2;//ref to other name text obj

    public GameObject Image_Rect;

    public int cardIndex;
    public Vector3 targetPosition;//position the card is moving to 
    public Vector3 offsetPosition;//offsets from that position (used to make hovering over a card move it up)
    public bool IsPlayerCard = false;//if the card can be clicked on and played when its the player turn
    public bool IsFaceDown = true;//is the card face down
    public bool followingCursor;//used by mouse control
    public AbilityType Ability = AbilityType.None;
    void Start()
    {

        //default pos is 666, so if its the default then set to to current so we dont move
        if (targetPosition.y == 666f)
        {
            targetPosition = transform.position;
        }

    }
    public void SetSortingLayer(int layer)
    {
        int trueLayer = layer * 8;//space them out cuz each card uses a lot of layers
        GetComponent<SpriteRenderer>().sortingOrder = trueLayer;
        
        
        Image_Rect.GetComponent<SpriteRenderer>().sortingOrder = trueLayer + 1;

        BorderOutline.GetComponent<SpriteRenderer>().sortingOrder = trueLayer + 2;

        HoverOverlay.GetComponent<SpriteRenderer>().sortingOrder = trueLayer + 3;


        NameText.GetComponent<TextMeshPro>().sortingOrder = trueLayer + 4;
        NameText2.GetComponent<TextMeshPro>().sortingOrder = trueLayer + 4;
        AbilityText.GetComponent<TextMeshPro>().sortingOrder = trueLayer + 4;
        FlavorText.GetComponent<TextMeshPro>().sortingOrder = trueLayer + 4;


        BorderColor.GetComponent<SpriteRenderer>().sortingOrder = trueLayer + 5;
        TagImageOutline.GetComponent<SpriteRenderer>().sortingOrder = trueLayer + 6;
        TagImage.GetComponent<SpriteRenderer>().sortingOrder = trueLayer + 7;


        setIsFaceDown(IsFaceDown);
    }
    //handles the layering of the back card sprite.
    public void setIsFaceDown(bool isFaceDown)
    {
        IsFaceDown = isFaceDown;
        if (isFaceDown)
        {
            CardBack.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder+8;
        }
        else
        {
            CardBack.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        }
    }
    //copies the data from a carddata object onto this card.
    public void copyFromCardData(Card_Manager.CardData cardData)
    {
        TextMeshPro txt = NameText.GetComponent<TextMeshPro>();
        txt.text = cardData.Name;
        txt = NameText2.GetComponent<TextMeshPro>();
        txt.text = cardData.Name;

        txt = FlavorText.GetComponent<TextMeshPro>();
        txt.text = cardData.FlavorText;

        SpriteRenderer spr = Image_Rect.GetComponent<SpriteRenderer>();
        spr.sprite = cardData.CardImage;

        Tags.Clear();
        for (int i = 0; i < cardData.Tags.Length; i++)
        {
            Tags.Add(cardData.Tags[i]);
        }
        Ability = cardData.AbilityIndex;


        txt = AbilityText.GetComponent<TextMeshPro>();
        txt.text = TextForAbilities[(int) cardData.AbilityIndex];

        updateTagImages();
    }
    //adds tags, auto updates text.
    public void addTag(TagType tagToAdd)
    {
        for (int i = 0; i < Tags.Count; i++)
        {
            if (Tags[i] == tagToAdd)
            {
                return;
            }
        }
        Tags.Add(tagToAdd);
        updateTagImages();
    }

    //Adds a card back to the deck
    public void Discard()
    {
        Card_Manager m = manager.GetComponent<Card_Manager>();
        m.RemoveFromAllLists(gameObject);
        m.Deck.Add(gameObject);
        targetPosition = new Vector3(2, 0, 0);
        setIsFaceDown(true);
        m.ShuffleDeck();
    }
    //clears all card tags, auto updates text
    public void clearTags()
    {
        Tags.Clear();

        //TextMeshPro txt = TagText.GetComponent<TextMeshPro>();
        //string newText = "";
        //txt.text = newText;
    }
    //updates the text
    public void updateTagImages()
    {
        //TextMeshPro txt = TagText.GetComponent<TextMeshPro>();
        //string newText = "";
        //newText = TagNames[(int)Tags[0]];
        for (int i = 0; i < Tags.Count; i++)
        {
            if (TagIsSecondary[(int)Tags[i]])
            {
                Sprite tag = Resources.Load<Sprite>("Images/Elements/" + TagNames[(int)Tags[i]]);
                SpriteRenderer spr = TagImage.GetComponent<SpriteRenderer>();
                spr.sprite = tag;
            } else
            {
                SpriteRenderer spr = BorderColor.GetComponent<SpriteRenderer>();
                switch (Tags[i])
                {
                    case TagType.Earth:
                        spr.color = new Color(0.1490f, 0.8235f, 0.13725f);
                        break;
                    case TagType.Fire:
                        spr.color = new Color(0.9529f, 0.2745f, 0.239215f);
                        break;
                    case TagType.Water:
                        spr.color = new Color(0.3922f, 0.70588f, 1f);
                        break;
                    case TagType.Air:
                        spr.color = new Color(1f, .9607f, 0.690196f);
                        break;
                    case TagType.Wild:
                    default:
                        spr.color = new Color(0f, 0f, 0f);
                        break;
                }   
            }
            //newText = newText + " " + TagNames[(int)Tags[i]];
        }
        //txt.text = newText;
    }
    //returns if a card shares a tag
    public bool CheckIfSharesTag(List<TagType> OtherCardTags, bool AllowWilds = true)
    {

        for (int i = 0; i < Tags.Count; i++)
        {
            for (int j = 0; j < OtherCardTags.Count; j++)
            {
                if (Tags[i] == OtherCardTags[j] || (Tags[i] == TagType.Wild && AllowWilds))
                {
                    return true;
                }
            }
        }
        return false;
    }

    //checks if shares tag with last played card
    public bool CanBePlayed()
    {
        if (manager.GetComponent<Card_Manager>().LastPlayedCard != null)
        {
            Card_Manager.TurnType currentTurnType = manager.GetComponent<Card_Manager>().CurrentTurnType;
            Card_Script lastCardScript = manager.GetComponent<Card_Manager>().LastPlayedCard.GetComponent<Card_Script>();

            if (currentTurnType == Card_Manager.TurnType.GivingCard)
            {

                return true;

            }
            if (currentTurnType == Card_Manager.TurnType.Discard1Card)
            {

                return true;

            }
            if (currentTurnType == Card_Manager.TurnType.Normal)
            {
                return CheckIfSharesTag(lastCardScript.Tags);
            }
            if (currentTurnType == Card_Manager.TurnType.Draw2)
            {
                if (Ability == AbilityType.Draw2 || Ability == AbilityType.Block)
                {
                    return CheckIfSharesTag(lastCardScript.Tags, false);
                }

            }

        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Alm)
        {


            //manage the offset when hovering over a playable card. also handle playing on click
            offsetPosition = new Vector3(0, 0, 0);

            if (CanBePlayed() && IsPlayerCard && manager.GetComponent<Card_Manager>().currentTurn == 0)
            {
                Vector3 topCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x + 0.6f, targetPosition.y + 1.15f, 0.0f));
                Vector3 botCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x - 0.6f, targetPosition.y - 1.15f, 0.0f));

                if ((Input.mousePosition.x < topCor.x && Input.mousePosition.x > botCor.x) && (Input.mousePosition.y < topCor.y && Input.mousePosition.y > botCor.y))
                {
                    offsetPosition = new Vector3(0, .3f, 0);
                    hoverOverlayAlpha = Mathf.Min(hoverOverlayAlpha + .05f, 1.0f);
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (manager.GetComponent<Card_Manager>().mouseControl)
                        {
                            SetSortingLayer(300);
                            followingCursor = true;
                        }
                        else
                        {
                            manager.GetComponent<Card_Manager>().PlayCard(gameObject, 0);
                        }


                    }
                } else
                {
                    hoverOverlayAlpha = Mathf.Max(hoverOverlayAlpha - .04f, 0.0f);

                }
            }
            else if (IsPlayerCard)
            {
                offsetPosition = new Vector3(0, -.3f, 0);
                Vector3 topCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x + 0.6f, targetPosition.y + 1.15f, 0.0f));
                Vector3 botCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x - 0.6f, targetPosition.y - 1.15f, 0.0f));

                if ((Input.mousePosition.x < topCor.x && Input.mousePosition.x > botCor.x) && (Input.mousePosition.y < topCor.y && Input.mousePosition.y > botCor.y))
                {
                    hoverOverlayAlpha = Mathf.Min(hoverOverlayAlpha + .04f, 1.0f);
                    offsetPosition = new Vector3(0, -.2f, 0);
                } else
                {
                    hoverOverlayAlpha = Mathf.Max(hoverOverlayAlpha - .04f, 0.0f);
                }
            } else
            {
                if (!(IsFaceDown))
                {
                    Vector3 topCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x + 0.6f, targetPosition.y + 1.15f, 0.0f));
                    Vector3 botCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x - 0.6f, targetPosition.y - 1.15f, 0.0f));

                    if ((Input.mousePosition.x < topCor.x && Input.mousePosition.x > botCor.x) && (Input.mousePosition.y < topCor.y && Input.mousePosition.y > botCor.y))
                    {
                        hoverOverlayAlpha = Mathf.Min(hoverOverlayAlpha + .04f, 1.0f);

                    }
                    else
                    {
                        hoverOverlayAlpha = Mathf.Max(hoverOverlayAlpha - .04f, 0f);
                    }
                } else
                {
                    hoverOverlayAlpha = Mathf.Max(hoverOverlayAlpha - .04f, 0.0f);
                }
                    
            }
            if (followingCursor)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(
                transform.position.x - (transform.position.x - (mousePos.x)) * .08f,
                transform.position.y - (transform.position.y - (mousePos.y)) * .08f,
                0
                );
                if (Input.GetMouseButtonUp(0))
                {
                    followingCursor = false;
                    manager.GetComponent<Card_Manager>().updatePlayerCardPositions();
                    if (IsPlayerCard)
                    {
                        if (mousePos.x > -1.1f && mousePos.x < 1.1f)
                        {
                            if (mousePos.y > -2.1f && mousePos.y < 2.1f)
                            {
                                manager.GetComponent<Card_Manager>().PlayCard(gameObject, 0);
                            }
                        }
                    }
                    else
                    {
                        SetSortingLayer(300);
                        if (mousePos.y < -2.8)
                        {
                            manager.GetComponent<Card_Manager>().PlayerDrawCards(0, manager.GetComponent<Card_Manager>().GetTopCard());
                        }
                    }

                }
            }
            else
            {

                transform.position = new Vector3(
                transform.position.x - (transform.position.x - (targetPosition.x + offsetPosition.x)) * .03f,
                transform.position.y - (transform.position.y - (targetPosition.y + offsetPosition.y)) * .03f,
                transform.position.z - (transform.position.z - (targetPosition.z + offsetPosition.z)) * .03f
                );
                
            }
        } else
        {
            Vector3 topCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x + 0.6f, targetPosition.y + 1.15f, 0.0f));
            Vector3 botCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x - 0.6f, targetPosition.y - 1.15f, 0.0f));
            offsetPosition = new Vector3(0, 0, 0);
            if ((Input.mousePosition.x < topCor.x && Input.mousePosition.x > botCor.x) && (Input.mousePosition.y < topCor.y && Input.mousePosition.y > botCor.y))
            {
                offsetPosition = new Vector3(0, .1f, 0);
                hoverOverlayAlpha = Mathf.Min(hoverOverlayAlpha + .05f, 1.0f);
            } else
            {
                hoverOverlayAlpha = Mathf.Max(hoverOverlayAlpha - .04f, 0.0f);
            }
        }
        HoverOverlay.GetComponent<SpriteRenderer>().color = new Color(0,0,0, hoverOverlayAlpha * .75f);

        FlavorText.GetComponent<TextMeshPro>().color = new Color(1f, 1f, 1f, hoverOverlayAlpha *.75f);
        FlavorText.GetComponent<TextMeshPro>().outlineColor = new Color(1f, 1f, 1f, hoverOverlayAlpha * .75f);

        AbilityText.GetComponent<TextMeshPro>().color = new Color(1f, 1f, 1f, hoverOverlayAlpha * .75f);
        AbilityText.GetComponent<TextMeshPro>().outlineColor = new Color(1f, 1f, 1f, hoverOverlayAlpha * .75f);
        //move 2 target pos 4 smoother anims




    }

    public void setPos(float x, float y)
    {
        targetPosition = new Vector3(x, y, transform.position.z);
        transform.position = new Vector3(x, y, transform.position.z);
    }
    public Vector2 getPos()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

}
