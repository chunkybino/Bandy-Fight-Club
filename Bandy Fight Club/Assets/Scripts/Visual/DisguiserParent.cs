using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DisguiserParent : MonoBehaviour
{
    public int disguiseLevel;

    public static int disguiseLevelStatic;
    public List<Disguiser> disguiserList = new List<Disguiser>();

    public bool doSilhouette;
    public static bool doSilhouetteStatic;

    public bool hideQWeap;
    public static bool hideQWeapStatic;
    public bool hideJWeap;
    public static bool hideJWeapStatic;
    public bool hideMWeap;
    public static bool hideMWeapStatic;

    int oldLevel;
    bool oldSilhouette;

    public bool removeNull;

    void Awake()
    {
        FindDisguise();
    }
    void Start()
    {
        if (GameManager.singleton) GameManager.singleton.onRespawn.AddListener(OnRespawn);

        oldLevel = disguiseLevel;
        CheckDisguise();
    }
    void OnValidate()
    {
        disguiseLevelStatic = disguiseLevel;
        doSilhouetteStatic = doSilhouette;

        hideQWeapStatic = hideQWeap;
        hideJWeapStatic = hideJWeap;
        hideMWeapStatic = hideMWeap;

        if (oldLevel == disguiseLevel && oldSilhouette == doSilhouette) return;

        oldLevel = disguiseLevel;
        oldSilhouette = doSilhouette;

        CheckDisguise();

        if (removeNull)
        {
            removeNull = false;
            disguiserList = UFunc.RemoveNullsList(disguiserList);
        }
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
            if (d == null) continue;
            d.CheckDisguise();
        }
    }

    void OnRespawn()
    {
        disguiserList = UFunc.RemoveNullsList(disguiserList);
    }
}
