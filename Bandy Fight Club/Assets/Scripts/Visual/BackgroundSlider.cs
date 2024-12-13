using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSlider : MonoBehaviour
{

    [SerializeField] float speed = 1;

    [SerializeField] Transform object1;
    [SerializeField] Transform object2;

    [SerializeField] float distance;

    void Awake()
    {
        distance = Mathf.Abs(object1.position.x - object2.position.x);
    }

    // Update is called once per frame
    void Update()
    {
        object1.position = new Vector2(object1.position.x + speed*Time.deltaTime, object1.position.y);
        object2.position = new Vector2(object2.position.x + speed*Time.deltaTime, object2.position.y);

        if (object1.position.x >= distance)
        {
            object1.position = new Vector2(object2.position.x-distance, object1.position.y);
        }

        if (object2.position.x >= distance)
        {
            object2.position = new Vector2(object1.position.x-distance, object2.position.y);
        }
    }
}
