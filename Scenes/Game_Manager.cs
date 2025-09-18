using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public enum GameState {
        MainMenu,
        Settings,
        Almanac,
        PlayingGame,
        PostGame,
    }
    public GameObject[] MainMenuObjects;
    public MouseRect[] MainMenuRects;
    public MouseRect[] ActiveRects;
    public GameObject Background;
    public float BackgroundTick;
    public GameObject CardManager;
    public GameState State;

    public class MouseRect
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public bool pressed = false;
        public bool performAction = false;
        public MouseRect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public bool isHovering()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos.x < x+width/2 && mousePos.x > x - width / 2)
            {
                if (mousePos.y < y + height / 2 && mousePos.y > y - height / 2)
                {
                    return true;
                }

            }
            return false;
        }
        public void UpdateRect()
        {
            
            if (performAction)
            {
                performAction = false;
            }
            Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(x-width/2,y-height/2,0));
            Vector3 pos2 = Camera.main.WorldToScreenPoint(new Vector3(x + width / 2, y + height / 2, 0));
            if (Input.mousePosition.x < pos2.x && Input.mousePosition.x > pos.x)
            {
                if (Input.mousePosition.y < pos2.y && Input.mousePosition.y > pos.y)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        pressed = true;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        performAction = true;
                        pressed = false;
                    }
                }

            }
            if (Input.GetMouseButtonUp(0))
            {
                pressed = false;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        MainMenuRects = new MouseRect[] {
            new MouseRect(-5.9f, 0.7f, 20f, 10f)
        };
        Background.GetComponent<SpriteRenderer>().sortingOrder = 800;
        SetGameState(GameState.MainMenu);
    }
    void putAllObjectsInTheBack()
    {
        Background.GetComponent<SpriteRenderer>().sortingOrder = -9999;
        for (int i = 0; i < MainMenuObjects.Length; i++)
        {
            MainMenuObjects[i].GetComponent<SpriteRenderer>().sortingOrder = -10000;
        }
    }
    void SetGameState(GameState state)
    {
        putAllObjectsInTheBack();
        ActiveRects = new MouseRect[] { };
        if (state == GameState.MainMenu)
        {
            ActiveRects = MainMenuRects;
            Background.GetComponent<SpriteRenderer>().sortingOrder = 800;
            for (int i = 0; i < MainMenuObjects.Length; i++)
            {
                MainMenuObjects[i].GetComponent<SpriteRenderer>().sortingOrder = 1000;
            }
        }
        State = state;
    }
    void StartGame()
    {
        Background.GetComponent<SpriteRenderer>().sortingOrder = -1000;
        CardManager.GetComponent<Card_Manager>().StartNewGame();
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0;i < ActiveRects.Length;i++)
        {
            ActiveRects[i].UpdateRect(); 
        }
        //Debug.Log(ActiveRects[0]);
        if (State == GameState.MainMenu)
        {
            if (ActiveRects[0].performAction)
            {
                StartGame();
                SetGameState(GameState.PlayingGame);

            }
        }

        BackgroundTick += Time.deltaTime;
        Background.transform.position = new Vector3(BackgroundTick % 1.28f, (Mathf.Sin(BackgroundTick) + BackgroundTick / 2) % 1.28f, 0);

        //Vector3 topCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x + 0.6f, targetPosition.y + 1.15f, 0.0f));
        //Vector3 botCor = Camera.main.WorldToScreenPoint(new Vector3(targetPosition.x - 0.6f, targetPosition.y - 1.15f, 0.0f));

        //if ((Input.mousePosition.x < topCor.x && Input.mousePosition.x > botCor.x) && (Input.mousePosition.y < topCor.y && Input.mousePosition.y > botCor.y))
        //{

        //}
    }
}
