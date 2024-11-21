using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] InputMapSO[] inputMaps;

    public int[] selectedCharacters;
    int characterCount;

    public TextMeshProUGUI[] characterSelectText;

    public string[] characterNames;

    public int menuIndex = 0; //the menu we're in

    public int selectPos; //the button we're selecting
    [SerializeField] GameObject mainMenu;
    [SerializeField] int mainMenuIndex = 0;
    [SerializeField] int mainMenuSelectionOptions = 2;

    [SerializeField] GameObject characterSelect;
    [SerializeField] int characterSelectMenuIndex = 1;

    void Awake()
    {
        gameManager = GameManager.singleton;
    }

    void Start()
    {
        inputMaps = gameManager.inputMaps;
        characterCount = gameManager.characterList.Length;

        selectedCharacters = new int[inputMaps.Length];

        for (int i = 0; i < characterSelectText.Length; i++)
        {
            characterSelectText[i].color = gameManager.teamOutlineColors.colors[i];
        }
    }

    void Update()
    {
        if (!gameManager.isMenu) return;

        mainMenu.SetActive(menuIndex == mainMenuIndex);
        characterSelect.SetActive(menuIndex == characterSelectMenuIndex);

        if (menuIndex == mainMenuIndex)
        {
            if (Input.GetKeyDown(inputMaps[0].up)) selectPos++; //the wall
            if (Input.GetKeyDown(inputMaps[1].up)) selectPos++;
            if (Input.GetKeyDown(inputMaps[2].up)) selectPos++;
            if (Input.GetKeyDown(inputMaps[0].down)) selectPos--;
            if (Input.GetKeyDown(inputMaps[1].down)) selectPos--;
            if (Input.GetKeyDown(inputMaps[2].down)) selectPos--;

            if (selectPos < 0) selectPos = mainMenuSelectionOptions-1;
            if (selectPos >= mainMenuSelectionOptions) selectPos = 0;

            if (Input.GetKeyDown("space")) SelectPlayerAmount(selectPos == 1); //do 3 player if we selected 1
        }
        else if (menuIndex == characterSelectMenuIndex)
        {
            characterSelectText[2].gameObject.SetActive(gameManager.do3Player);

            for (int i = 0; i < selectedCharacters.Length; i++)
            {
                int num = selectedCharacters[i];

                if (Input.GetKeyDown(inputMaps[i].up)) num++;
                if (Input.GetKeyDown(inputMaps[i].down)) num--;

                if (num < 0) num = characterCount-1;
                if (num > characterCount-1 ) num = 0;

                selectedCharacters[i] = num;

                characterSelectText[i].text = characterNames[num];
            }

            if (Input.GetKeyDown("space"))
            {
                gameManager.StartMatchTransition();
            }
        }
    }

    void SelectPlayerAmount(bool do3Player)
    {
        gameManager.do3Player = do3Player;
        gameManager.DoPlayerInfo();

        menuIndex = characterSelectMenuIndex;
    }
}
