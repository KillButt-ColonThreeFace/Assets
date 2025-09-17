using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Card_Script : MonoBehaviour
{
    //enum for the tag types
    public enum TagType
    {
        Plant, Creature, Person, Object,//"main" tags
        Gross, Real, Cute, Spooky, Sneaky, Digital, Cool, Melancholic, Magical, Evil, Bald,//"seconday" tags

        Devious,Jolly//"special" tags (used as 1-offs for some special cards)
    }
    //enum for the abilities
    public enum AbilityType
    {
        None,Skip, KillTheOtherPlayer,Draw2,Draw4,Reverse
    }
    //list of the tag names (used for auto-generating the tag text)
    private static string[] TagNames = new string[] {
        "Plant", "Creature", "Person", "Object",
        "Gross", "Real", "Cute", "Spooky", "Sneaky", "Digital", "Cool", "Melancholic", "Magical", "Evil","Bald",

        "Devious","Jolly!"
    };
    public Card_Manager manager;//reference to card manager object
    public TagType[] Tags;//what tags the card has
    public GameObject TagText;//ref to the tag text object
    public GameObject DescriptionText;//ref to the desctiption text object
    public GameObject NameText;//ref to the name text object
    public GameObject Image_Rect;//ref to image object
    public GameObject TagDivider;//ref to little gray rec on the card text
    public GameObject Overlay;//ref to the card border
    public GameObject CardBack;//ref to the back of the card
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
        int trueLayer = layer * 6;//space them out cuz each card uses a lot of layers
        GetComponent<SpriteRenderer>().sortingOrder = trueLayer;
        Image_Rect.GetComponent<SpriteRenderer>().sortingOrder = trueLayer+1;

        TagDivider.GetComponent<SpriteRenderer>().sortingOrder = trueLayer+2;
        TagText.GetComponent<TextMeshPro>().sortingOrder = trueLayer+2;
        DescriptionText.GetComponent<TextMeshPro>().sortingOrder = trueLayer+2;
        NameText.GetComponent<TextMeshPro>().sortingOrder = trueLayer+2;

        Overlay.GetComponent<SpriteRenderer>().sortingOrder = trueLayer+2;
        setIsFaceDown(IsFaceDown);
    }
    //handles the layering of the back card sprite.
    public void setIsFaceDown(bool isFaceDown)
    {
        IsFaceDown = isFaceDown;
        if (isFaceDown )
        {
            CardBack.GetComponent<SpriteRenderer>().sortingOrder = Overlay.GetComponent<SpriteRenderer>().sortingOrder+1;
        } else
        {
            CardBack.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder -1;
        }
    }
    //copies the data from a carddata object onto this card.
    public void copyFromCardData(Card_Manager.CardData cardData)
    {


        TextMeshPro txt = NameText.GetComponent<TextMeshPro>();
        txt.text = cardData.Name;
        txt = DescriptionText.GetComponent<TextMeshPro>();
        txt.text = cardData.Description;

        SpriteRenderer spr = Image_Rect.GetComponent<SpriteRenderer>();
        spr.sprite = cardData.CardImage;
        
        Tags = cardData.Tags;
        Ability = cardData.AbilityIndex;
        updateTagsText();
    }
    //adds tags, auto updates text.
    public void addTag(TagType tagToAdd)
    {
        for (int i = 0; i < Tags.Length; i++)
        {
            if (Tags[i] == tagToAdd)
            {
                return;
            }
        }
        ArrayUtility.Add(ref Tags, tagToAdd);
        updateTagsText();
    }
    //clears all card tags, auto updates text
    public void clearTags()
    {
        ArrayUtility.Clear(ref Tags);
        
        TextMeshPro txt = TagText.GetComponent<TextMeshPro>();
        string newText = "";
        txt.text = newText;
    }
    //updates the text
    public void updateTagsText()
    {
        TextMeshPro txt = TagText.GetComponent<TextMeshPro>();
        string newText = "";
        newText = TagNames[(int)Tags[0]];
        for (int i=1; i < Tags.Length;i++)
        {
            newText = newText + " " + TagNames[(int)Tags[i]];
        }
        txt.text = newText;
    }
    //returns if a card shares a tag
    public bool CheckIfSharesTag(TagType[] OtherCardTags)
    {

        for (int i = 0; i < Tags.Length;i++)
        {
            for (int j = 0; j < OtherCardTags.Length;j++)
            {
                if (Tags[i] == OtherCardTags[j])
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
            Card_Script lastCardScript = manager.GetComponent<Card_Manager>().LastPlayedCard.GetComponent<Card_Script>();

            if (CheckIfSharesTag(lastCardScript.Tags))
            {
                return true;
            }

        }

        return false; 
    }

    // Update is called once per frame
    void Update()
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
                if (Input.GetMouseButtonDown(0))
                {
                    if (manager.GetComponent<Card_Manager>().mouseControl)
                    {
                        followingCursor = true;
                    } else
                    {
                        manager.GetComponent<Card_Manager>().PlayCard(gameObject, 0);
                    }
                        
                    
                }
            }
        }
        else if (IsPlayerCard)
        {
            offsetPosition = new Vector3(0, -.3f, 0);
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
                if (IsPlayerCard)
                {
                    if (mousePos.x > -1.1f && mousePos.x < 1.1f)
                    {
                        if (mousePos.y > -2.1f && mousePos.y < 2.1f)
                        {
                            manager.GetComponent<Card_Manager>().PlayCard(gameObject, 0);
                        }
                    }
                } else
                {
                    if (mousePos.y < .01) {
                        manager.GetComponent<Card_Manager>().DrawNewCard(0);
                        manager.GetComponent<Card_Manager>().MoveToNextTurn();
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


        //move 2 target pos 4 smoother anims
        
        


    }

}
