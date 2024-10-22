using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DisguiserParent : MonoBehaviour
{
    public int disguiseLevel;

    public static int disguiseLevelStatic;
    public List<Disguiser> disguiserList = new List<Disguiser>();

    int oldLevel;

    void Awake()
    {
        FindDisguise();
    }
    void Start()
    {
        oldLevel = disguiseLevel;
        CheckDisguise();
    }
    void OnValidate()
    {
        disguiseLevelStatic = disguiseLevel;

        if (oldLevel == disguiseLevel) return;

        oldLevel = disguiseLevel;

        CheckDisguise();
    }

    public void FindDisguise()
    {
        disguiserList.Clear();
        disguiserList = UFunc.ArrayToList(FindObjectsOfType<Disguiser>(true));
    }
    public void AddDisguise(Disguiser d)
    {
        if (!disguiserList.Contains(d)) disguiserList.Add(d);
    }

    void CheckDisguise()
    {
        foreach (Disguiser d in disguiserList)
        {
            d.CheckDisguise();
        }
    }
}
