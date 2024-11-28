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
    [SerializeField] int mainMenuIndex = 0;
    [SerializeField] int mainMenuSelectionOptions = 2;

    [SerializeField] Vector4[] mainMenuButtonPosSize;
    [SerializeField] RectTransform[] mainMenuButtons;
    [SerializeField] float timeSinceSinceButtonSelect;
    [SerializeField] float buttonSelectAnimSpeed = 1;
    [SerializeField] AnimationCurve buttonSelectAnimCurve;
    [SerializeField] RectTransform buttonSelectionObject;
    [SerializeField] Vector4 lastButtonPosSize;
    [SerializeField] Vector4 targetButtonPosSize;

    [SerializeField] GameObject characterSelect;
    [SerializeField] int characterSelectMenuIndex = 1;

    public List<InputMapSO> playerOrderedInputMaps = new List<InputMapSO>(); //press a button to join

    float timeEscapeHeld;
    [SerializeField] float exitToMainMenuEscapeHoldTime = 1.5f;

    ControllerInput controller;
    /*InputAction upAction;
    InputAction downAction;
    InputAction leftAction;
    InputAction rightAction;
    InputAction jumpAction;
    InputAction dodgeAction;
    InputAction attackAction;
    InputAction heavyAction;

    InputAction startAction;
    InputAction backAction;*/

    void Awake()
    {
        gameManager = GameManager.singleton;

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

        timeSinceSinceButtonSelect = 1;

        controller = new ControllerInput();
        /*upAction = controller.Gameplay.Up;
        downAction = controller.Gameplay.Down;
        leftAction = controller.Gameplay.Left;
        rightAction = controller.Gameplay.Right;
        jumpAction = controller.Gameplay.Jump;
        dodgeAction = controller.Gameplay.Dodge;
        attackAction = controller.Gameplay.Attack;
        heavyAction = controller.Gameplay.Heavy;
        startAction = controller.Gameplay.Start;
        backAction = controller.Gameplay.Back;*/

        //controller.Gameplay.Enable();
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
    }

    void Update()
    {
        //if (!gameManager.isMenu) return;

        mainMenu.SetActive(menuIndex == mainMenuIndex);
        buttonSelectionObject.gameObject.SetActive(menuIndex == mainMenuIndex);
        characterSelect.SetActive(menuIndex == characterSelectMenuIndex);

        if (!gameManager.isMenu) //in game
        {
            if (GetBackInputHold()) {
                timeEscapeHeld += Time.deltaTime;
            } else {
                timeEscapeHeld = 0;
            }

            if (timeEscapeHeld >= exitToMainMenuEscapeHoldTime)
            {
                timeEscapeHeld = 0;
                gameManager.ExitMatch();
            }
        }
        else
        {
            if (menuIndex == mainMenuIndex)
            {
                MainMenuFunc();
            }
            else if (menuIndex == characterSelectMenuIndex)
            {
                CharacterSelectFunc();
            }
        }
    }

    void MainMenuFunc()
    {
        timeSinceSinceButtonSelect += Time.deltaTime * buttonSelectAnimSpeed;

        float t = buttonSelectAnimCurve.Evaluate(timeSinceSinceButtonSelect);

        buttonSelectionObject.anchoredPosition = new Vector2(
            Mathf.Lerp(lastButtonPosSize.x, mainMenuButtonPosSize[selectPos].x, t),
            Mathf.Lerp(lastButtonPosSize.y, mainMenuButtonPosSize[selectPos].y, t)
        );
        buttonSelectionObject.sizeDelta = new Vector2(
            Mathf.Lerp(lastButtonPosSize.z, mainMenuButtonPosSize[selectPos].z, t),
            Mathf.Lerp(lastButtonPosSize.w, mainMenuButtonPosSize[selectPos].w, t)
        );

        int add = 0;
        for (int i = 0; i < inputMaps.Length; i++)
        {
            if (GetUpInput(inputMaps[i])) add = 1;
            if (GetDownInput(inputMaps[i])) add = -1;
        }

        if (add != 0)
        {
            selectPos += add;
            timeSinceSinceButtonSelect = 0;
            lastButtonPosSize = new Vector4( 
                buttonSelectionObject.anchoredPosition.x,
                buttonSelectionObject.anchoredPosition.y,
                buttonSelectionObject.sizeDelta.x,
                buttonSelectionObject.sizeDelta.y
            );
        }

        if (selectPos < 0) selectPos = mainMenuSelectionOptions-1;
        if (selectPos >= mainMenuSelectionOptions) selectPos = 0;

        if (add != 0) targetButtonPosSize = mainMenuButtonPosSize[selectPos]; //do after we've looped select pos

        if (GetStartInput()) SelectPlayerAmount(selectPos == 1); //do 3 player if we selected 1
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
        }
        if (GetBackInput()) ToMainMenu(); 
    }

    void SelectPlayerAmount(bool do3Player)
    {
        int num = 2;
        if (do3Player) num = 3;

        gameManager.do3Player = do3Player;

        playerOrderedInputMaps.Clear();
        selectedCharacters = new int[num];

        gameManager.DoPlayerInfo();

        menuIndex = characterSelectMenuIndex;
    }

    public void ToGame()
    {
        menuIndex = -1;
    }
    public void ToMainMenu()
    {
        menuIndex = mainMenuIndex;
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
}
