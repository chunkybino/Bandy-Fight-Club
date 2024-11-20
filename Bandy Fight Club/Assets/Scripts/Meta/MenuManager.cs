using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] InputMapSO[] inputMaps;

    public int[] selectedCharacters;
    int characterCount;

    void Awake()
    {
        gameManager = GameManager.singleton;
    }

    void Start()
    {
        inputMaps = gameManager.inputMaps;
        characterCount = gameManager.characterList.Length;

        selectedCharacters = new int[inputMaps.Length];
    }

    void Update()
    {
        if (!gameManager.isMenu) return;

        for (int i = 0; i < selectedCharacters.Length; i++)
        {
            int num = selectedCharacters[i];

            if (Input.GetKeyDown(inputMaps[i].up)) num++;
            if (Input.GetKeyDown(inputMaps[i].down)) num--;

            if (num < 0) num = characterCount-1;
            if (num > characterCount-1 ) num = 0;

            selectedCharacters[i] = num;
        }
    }
}
