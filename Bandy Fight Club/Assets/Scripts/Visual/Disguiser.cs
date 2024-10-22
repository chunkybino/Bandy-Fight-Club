using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disguiser : MonoBehaviour
{
    DisguiserParent parent;

    public int enableLevel; //enable if disguise level is less or equal this
    public bool inverse;

    public GameObject targetObject; //if null do self
    public GameObject inverseObject; //if null do self

    public void OnEnable()
    {
        CheckDisguise();

        parent = FindObjectOfType<DisguiserParent>();
        if (parent) parent.AddDisguise(this);
    }

    public void CheckDisguise()
    {
        if (targetObject == null) targetObject = gameObject;

        bool activate = DisguiserParent.disguiseLevelStatic <= enableLevel;
        if (inverse) activate = !activate;
        targetObject.SetActive(activate);
        if (inverseObject) inverseObject.SetActive(!activate);
    }
}
