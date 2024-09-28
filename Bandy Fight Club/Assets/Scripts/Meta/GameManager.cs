using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public float timeScale = 1;

    void Awake()
    {
        CheckSingleton();
    }
    void CheckSingleton()
    {
        if (!singleton) 
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckSingleton();

        Time.timeScale = timeScale;
    }
}
