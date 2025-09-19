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
    public GameObject MainMenuObjects;
    public GameObject SettingsObjects;
    public Sprite[] Backgrounds;
    public int BackgroundIndex = 0;
    public MouseRect[] MainMenuRects;
    public MouseRect[] SettingsRects;
    public MouseRect[] ActiveRects;
    public GameObject Background;
    public float BackgroundTick;
    public GameObject CardManager;
    public GameState State;
    private Sprite loadSpr(string name)//makes defining cards quicker
    {
        return Resources.Load<Sprite>("Backgrounds/" + name);
    }
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
             Debug.DrawLine(pos, pos2,Color.red,4f,false);
            Debug.DrawLine(new Vector3(x - width / 2, y - height / 2, 0), new Vector3(x + width / 2, y + height / 2, 0), Color.red, 4f, false);
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
            new MouseRect(0, -1.89f, 2.5f, 1.5f),
            new MouseRect(0, -3.89f, 5f, 1.5f)
        };
        SettingsRects = new MouseRect[] {
            new MouseRect(-7.6f,-4.89f,2.5f,1.5f),
            new MouseRect(3.16f,-0.26f,4.5f,1.5f),
        };
        Backgrounds = new Sprite[]
        {
            loadSpr("Stars"),
            loadSpr("poot"),
            loadSpr("gomi"),
            loadSpr("Dexter"),
            loadSpr("Rainbow_Bowser"),
            loadSpr("moxie"),
            loadSpr("garlic"),
            loadSpr("Dumb_Ways_to_Die"),
            loadSpr("cyber"),

        };

        Background.GetComponent<SpriteRenderer>().sortingOrder = 800;
        SetGameState(GameState.MainMenu);
    }
    void removeAllMenus()
    {
        Background.GetComponent<SpriteRenderer>().sortingOrder = -9999;
        MainMenuObjects.transform.position = new Vector3(1000,1000,0);
        SettingsObjects.transform.position = new Vector3(1000, 1000, 0);
    }
    void SetGameState(GameState state)
    {
        removeAllMenus();
        ActiveRects = new MouseRect[] { };
        if (state == GameState.MainMenu)
        {
            ActiveRects = MainMenuRects;
            Background.GetComponent<SpriteRenderer>().sortingOrder = 800;
            MainMenuObjects.transform.position = new Vector3(0, 0, 0);
        } else if (state == GameState.Settings)
        {
            Background.GetComponent<SpriteRenderer>().sortingOrder = 800;
            ActiveRects = SettingsRects;
            SettingsObjects.transform.position = new Vector3(0, 0, 0);
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
        if (ActiveRects.Length > 0) {
            for (int i = 0; i < ActiveRects.Length; i++)
            {
                ActiveRects[i].UpdateRect();
            }
        }

        //Debug.Log(ActiveRects[0]);
        if (State == GameState.MainMenu)
        {
            if (ActiveRects[0].performAction)
            {
                StartGame();
                SetGameState(GameState.PlayingGame);

            }
            else if (ActiveRects[1].performAction)
            {
                SetGameState(GameState.Settings);
            }

        }
        else if (State == GameState.Settings) {
            if (ActiveRects[0].performAction)
            {
                SetGameState(GameState.MainMenu);
            }
            else if (ActiveRects[1].performAction)
            {
                BackgroundIndex = (BackgroundIndex + 1) % Backgrounds.Length;
                Background.GetComponent<SpriteRenderer>().sprite = Backgrounds[BackgroundIndex];
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
