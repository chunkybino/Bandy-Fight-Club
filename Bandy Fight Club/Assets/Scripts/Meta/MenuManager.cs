using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] InputMapSO[] inputMaps;

    public int[] selectedCharacters;
    int characterCount;

    public TextMeshProUGUI[] characterSelectText;
    public Image[] characterSelectControllerIcons;
    public TextMeshProUGUI characterSelectStartText;
    public float characterSelectNotReadyOpacity;

    public string[] characterNames;

    public int menuIndex = 0; //the menu we're in

    public int selectPos; //the button we're selecting
    [SerializeField] GameObject mainMenu;
    [SerializeField] RectTransform mainMenuTransform;
    [SerializeField] int mainMenuIndex = 0;
    public bool reverseSelectionOrder;

    [SerializeField] Vector4[] mainMenuButtonPosSize;
    [SerializeField] RectTransform[] mainMenuButtons;
    [SerializeField] float timeSinceSinceButtonSelect;
    [SerializeField] float buttonSelectAnimSpeed = 1;
    [SerializeField] AnimationCurve buttonSelectAnimCurve;
    [SerializeField] RectTransform buttonSelectionObject;
    [SerializeField] Vector4 lastButtonPosSize;
    [SerializeField] Vector4 targetButtonPosSize;

    [SerializeField] int[] buttonSelectActiveIndexs; //menus where its active

    [SerializeField] GameObject characterSelect;
    [SerializeField] RectTransform characterSelectTransform;
    [SerializeField] int characterSelectMenuIndex = 1;

    [SerializeField] GameObject tutorialMenu;
    [SerializeField] RectTransform tutorialTransform;
    [SerializeField] int tutorialMenuIndex = 2;

    [SerializeField] Vector4[] tutorialButtonPosSize;
    [SerializeField] RectTransform[] tutorialButtons;

    public List<InputMapSO> playerOrderedInputMaps = new List<InputMapSO>(); //press a button to join

    [SerializeField] float exitMatchDoublePressTime = 1;
    float exitMatchDoublePressTimer = 1;
    [SerializeField] GameObject exitMatchEscIcon;

    ControllerInput controller;

    [SerializeField] AnimationCurve menuTransitionCurve;
    [SerializeField] Vector2 menuTransitionPos;
    [SerializeField] float menuTransitionSpeed = 1;
    float menuTransitionTimer = 2;
    [SerializeField] RectTransform prevMenuTransform;
    [SerializeField] RectTransform currentMenuTransform;
    int menuTransitionDirection;

    [SerializeField] GameObject menuSpotlights; //enalbe in menu
    [SerializeField] GameObject platforms; //disable in menu

    [SerializeField] AudioManager audio;

    [SerializeField] string menuButtonSoundName = "menuPress";
    [SerializeField] string[] menuMoveSoundName;
    int menuMoveSoundIndex;

    [SerializeField] GameObject[] tutorialSlides;

    void Awake()
    {
        gameManager = GameManager.singleton;

        mainMenuTransform = mainMenu.GetComponent<RectTransform>();
        characterSelectTransform = characterSelect.GetComponent<RectTransform>();
        tutorialTransform = tutorialMenu.GetComponent<RectTransform>();

        mainMenuButtonPosSize = new Vector4[mainMenuButtons.Length];
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtonPosSize[i] = new Vector4(
                mainMenuButtons[i].anchoredPosition.x,
                mainMenuButtons[i].anchoredPosition.y,
                mainMenuButtons[i].sizeDelta.x,
                mainMenuButtons[i].sizeDelta.y
            );
        }

        tutorialButtonPosSize = new Vector4[tutorialButtons.Length];
        for (int i = 0; i < tutorialButtons.Length; i++)
        {
            tutorialButtonPosSize[i] = new Vector4(
                tutorialButtons[i].anchoredPosition.x,
                tutorialButtons[i].anchoredPosition.y,
                tutorialButtons[i].sizeDelta.x,
                tutorialButtons[i].sizeDelta.y
            );
        }

        timeSinceSinceButtonSelect = 1;

        controller = new ControllerInput();
    }

    void Start()
    {
        inputMaps = gameManager.inputMaps;
        characterCount = gameManager.characterList.Length;

        selectedCharacters = new int[inputMaps.Length];

        for (int i = 0; i < characterSelectText.Length; i++)
        {
            characterSelectText[i].color = gameManager.teamOutlineColors.colors[i];
            characterSelectControllerIcons[i].color = gameManager.teamOutlineColors.colors[i];
        }

        controller.Gameplay.Enable();

        menuSpotlights.SetActive(true);
        platforms.SetActive(false);
    }

    void Update()
    {
        //if (!gameManager.isMenu) return;

        if (menuTransitionTimer > 1)
        {
            mainMenu.SetActive(menuIndex == mainMenuIndex);
            characterSelect.SetActive(menuIndex == characterSelectMenuIndex);
            tutorialMenu.SetActive(menuIndex == tutorialMenuIndex);

            /*buttonSelectionObject.gameObject.SetActive(false);
            for (int i = 0; i < buttonSelectActiveIndexs.Length; i++)
            {
                buttonSelectionObject.gameObject.SetActive(true);
            }*/
        }
        buttonSelectionObject.gameObject.SetActive(false);
        for (int i = 0; i < buttonSelectActiveIndexs.Length; i++)
        {
            if (buttonSelectActiveIndexs[i] == menuIndex) 
            {
                buttonSelectionObject.gameObject.SetActive(true);
                break;
            }
        } 

        if (!gameManager.isMenu) //in game
        {   
            exitMatchEscIcon.SetActive(exitMatchDoublePressTimer > 0);

            UFunc.TickTimer(ref exitMatchDoublePressTimer);
            if (GetBackInputPress())
            {
                if (exitMatchDoublePressTimer == 0)
                {
                    exitMatchDoublePressTimer = exitMatchDoublePressTime;
                }
                else
                {
                    exitMatchDoublePressTimer = 0;
                    exitMatchEscIcon.SetActive(false);
                    gameManager.ExitMatch();
                }
            }
        }
        else
        {
            if (menuTransitionTimer <= 1)
            {
                MenuTransitionFunc();
            }

            if (menuIndex == mainMenuIndex)
            {
                MainMenuFunc();
            }
            else if (menuIndex == characterSelectMenuIndex)
            {
                CharacterSelectFunc();
            }
            else if (menuIndex == tutorialMenuIndex)
            {
                TutorialFunc();
            }
        }
    }

    void MainMenuFunc()
    {
        ButtonSelectFunc(mainMenuButtonPosSize);

        if (GetStartInput()) 
        {
            if (selectPos == 0 || selectPos == 1)
            {
                SelectPlayerAmount(selectPos == 1); //do 3 player if we selected 1

                MenuPressSound();
            }
            if (selectPos == 2)
            {
                GoTutorial();

                MenuPressSound();
            }
            else if (selectPos == 3)
            {
                QuitGame();
            }
        }
    }
    void ButtonSelectFunc(Vector4[] posSize)
    {
        timeSinceSinceButtonSelect += Time.deltaTime * buttonSelectAnimSpeed;

        float t = buttonSelectAnimCurve.Evaluate(timeSinceSinceButtonSelect);

        buttonSelectionObject.anchoredPosition = new Vector2(
            Mathf.Lerp(lastButtonPosSize.x, posSize[selectPos].x, t),
            Mathf.Lerp(lastButtonPosSize.y, posSize[selectPos].y, t)
        );
        buttonSelectionObject.sizeDelta = new Vector2(
            Mathf.Lerp(lastButtonPosSize.z, posSize[selectPos].z, t),
            Mathf.Lerp(lastButtonPosSize.w, posSize[selectPos].w, t)
        );

        int add = 0;
        for (int i = 0; i < inputMaps.Length; i++)
        {
            if (GetUpInput(inputMaps[i])) add = 1;
            if (GetDownInput(inputMaps[i])) add = -1;
        }

        if (add != 0)
        {
            if(!reverseSelectionOrder)
            {
                selectPos += add;
            }
            else
            {
                selectPos -= add;
            }
            timeSinceSinceButtonSelect = 0;
            lastButtonPosSize = new Vector4( 
                buttonSelectionObject.anchoredPosition.x,
                buttonSelectionObject.anchoredPosition.y,
                buttonSelectionObject.sizeDelta.x,
                buttonSelectionObject.sizeDelta.y
            );

            MenuMoveSound();
        }

        if (selectPos < 0) selectPos = posSize.Length-1;
        if (selectPos >= posSize.Length) selectPos = 0;

        if (add != 0) targetButtonPosSize = posSize[selectPos]; //do after we've looped select pos
    }
    void GoInstantButtonSelect(Vector4 posSize)
    {
        timeSinceSinceButtonSelect = 1;

        buttonSelectionObject.anchoredPosition = new Vector2(posSize.x, posSize.y);
        buttonSelectionObject.sizeDelta = new Vector2(posSize.z, posSize.w);
    }

    void CharacterSelectFunc()
    {
        characterSelectText[2].gameObject.SetActive(gameManager.do3Player);

        for (int i = 0; i < selectedCharacters.Length; i++)
        {
            int num = selectedCharacters[i];

            if (i < playerOrderedInputMaps.Count) //if player has joined
            {
                if (GetUpInput(playerOrderedInputMaps[i])) num++;
                if (GetDownInput(playerOrderedInputMaps[i])) num++;

                if (num < 0) num = characterCount-1;
                if (num > characterCount-1) num = 0;

                selectedCharacters[i] = num;

                characterSelectText[i].text = characterNames[num];

                characterSelectControllerIcons[i].gameObject.SetActive(true);
            }
            else // if player has not joined
            {
                characterSelectText[i].text = ";)";

                characterSelectControllerIcons[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < inputMaps.Length; i++)
        {
            if (playerOrderedInputMaps.Contains(inputMaps[i])) continue;

            if (i == 3)
            {
                if (CheckControllerInput())
                {
                    Add();
                }
            }
            else
            {
                if (inputMaps[i].AnyInputPressed()) 
                {
                    Add();
                }
            }

            void Add()
            {
                playerOrderedInputMaps.Add(inputMaps[i]);
                characterSelectControllerIcons[playerOrderedInputMaps.Count-1].sprite = inputMaps[i].icon;
            }
        }

        bool CheckControllerInput()
        {
            if(controller.Gameplay.Up.IsPressed()) return true;
            if(controller.Gameplay.Down.IsPressed()) return true;
            if(controller.Gameplay.Left.IsPressed()) return true;
            if(controller.Gameplay.Right.IsPressed()) return true;
            if(controller.Gameplay.Jump.IsPressed()) return true;
            if(controller.Gameplay.Dodge.IsPressed()) return true;
            if(controller.Gameplay.Attack.IsPressed()) return true;
            if(controller.Gameplay.Heavy.IsPressed()) return true;
            return false;
        }

        bool canStart = false;
        if (playerOrderedInputMaps.Count >= selectedCharacters.Length) {
            canStart = true;
            Color c = characterSelectStartText.color;
            characterSelectStartText.color = new Vector4(c.r,c.g,c.b,1);
        } else {
            Color c = characterSelectStartText.color;
            characterSelectStartText.color = new Vector4(c.r,c.g,c.b,characterSelectNotReadyOpacity);
        }

        if (GetStartInput() && canStart)
        {
            gameManager.StartMatchTransition();

            MenuPressSound();
        }
        if (GetBackInput()) 
        {
            ToMainMenu(); 
            prevMenuTransform = characterSelectTransform;
            currentMenuTransform = mainMenuTransform;
            menuTransitionDirection = -1;
            DoMenuTransition();

            MenuPressSound();
        }
    }

    void TutorialFunc()
    {
        ButtonSelectFunc(tutorialButtonPosSize);

        if (GetBackInput()) 
        {
            ToMainMenu(); 
            prevMenuTransform = tutorialTransform;
            currentMenuTransform = mainMenuTransform;
            menuTransitionDirection = -1;
            DoMenuTransition();

            MenuPressSound();
        }

        for (int i = 0; i < tutorialSlides.Length; i++)
        {
            tutorialSlides[i].SetActive(i == selectPos);
        }
    }

    void SelectPlayerAmount(bool do3Player)
    {
        int num = 2;
        if (do3Player) num = 3;

        gameManager.do3Player = do3Player;

        playerOrderedInputMaps.Clear();
        selectedCharacters = new int[num];

        gameManager.DoPlayerInfo();

        prevMenuTransform = mainMenuTransform;
        currentMenuTransform = characterSelectTransform;
        menuTransitionDirection = 1;
        menuIndex = characterSelectMenuIndex;
        DoMenuTransition();
    }

    public void GoTutorial()
    {
        selectPos = 0;
        GoInstantButtonSelect(tutorialButtonPosSize[0]);

        prevMenuTransform = mainMenuTransform;
        currentMenuTransform = tutorialTransform;
        menuTransitionDirection = 1;
        menuIndex = tutorialMenuIndex;
        DoMenuTransition();
    }

    public void DoMenuTransition()
    {
        menuTransitionTimer = 0;

        prevMenuTransform.gameObject.SetActive(true);
        currentMenuTransform.gameObject.SetActive(true);
    }
    public void MenuTransitionFunc()
    {
        menuTransitionTimer += Time.deltaTime * menuTransitionSpeed;
        float t = menuTransitionCurve.Evaluate(menuTransitionTimer);

        Vector2 v = new Vector2(0,0);
        v.x = Mathf.Lerp(0, menuTransitionPos.x * menuTransitionDirection, t);
        v.y = Mathf.Lerp(0, menuTransitionPos.y * menuTransitionDirection, t);

        currentMenuTransform.anchoredPosition = v - menuTransitionPos * menuTransitionDirection;
        prevMenuTransform.anchoredPosition = v;
    }

    public void ToGame()
    {
        menuIndex = -1;

        menuSpotlights.SetActive(false);
        platforms.SetActive(true);
    }
    public void ToMainMenu()
    {
        selectPos = 0;
        GoInstantButtonSelect(mainMenuButtonPosSize[0]);

        menuIndex = mainMenuIndex;
        mainMenu.SetActive(true);
        currentMenuTransform = mainMenuTransform;
        currentMenuTransform.anchoredPosition = new Vector2(0,0);

        menuSpotlights.SetActive(true);
        platforms.SetActive(false);
    }

    public void QuitGame()
    {
        print("ya done did an exit");
        Application.Quit();
    }

    void MenuPressSound()
    {
        audio.Play(menuButtonSoundName, 1, 1);
    }
    void MenuMoveSound()
    {
        audio.Play(menuMoveSoundName[menuMoveSoundIndex], 1, 1);

        menuMoveSoundIndex++;
        if (menuMoveSoundIndex >= menuMoveSoundName.Length) menuMoveSoundIndex = 0;
    }

    bool GetUpInput(InputMapSO map)
    {
        if (map.doControllerInput)
        {
            return controller.Gameplay.Up.WasPressedThisFrame();
        }
        else
        {
            return Input.GetKeyDown(map.up);
        }
    }
    bool GetDownInput(InputMapSO map)
    {
        if (map.doControllerInput)
        {
            return controller.Gameplay.Down.WasPressedThisFrame();
        }
        else
        {
            return Input.GetKeyDown(map.down);
        }
    }
    public bool GetStartInput()
    {
        if (Input.GetKeyDown("space")) return true;
        if (controller.Gameplay.Start.WasPressedThisFrame()) return true;
        return false;
    }
    public bool GetBackInput()
    {
        if (Input.GetKeyDown("escape")) return true;
        if (controller.Gameplay.Back.WasPressedThisFrame()) return true;
        return false;
    }
    bool GetBackInputHold()
    {
        if (Input.GetKey("escape")) return true;
        if (controller.Gameplay.Back.IsPressed()) return true;
        return false;
    }
    bool GetBackInputPress()
    {
        if (Input.GetKeyDown("escape")) return true;
        if (controller.Gameplay.Back.WasPressedThisFrame()) return true;
        return false;
    }
}
