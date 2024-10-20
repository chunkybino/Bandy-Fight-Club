using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashStretcher : MonoBehaviour
{
    [SerializeField] bool doIt;

    [SerializeField] float time = 5;

    [SerializeField] float speed = 1;
    [SerializeField] float amp = 0.4f;
    [SerializeField] float freq = 1;
    [SerializeField] float pow = 2;

    [SerializeField] float perpAmp = 1f;

    [SerializeField] Vector2 scale = new Vector2(1,1);
    [SerializeField] Vector2 scaleMult = new Vector2(1,1);

    [SerializeField] Vector2 pivot = new Vector2(0,0);
    [SerializeField] bool doPivot;

    [SerializeField] bool flipXY;

    void Start()
    {
        time = 5;

        scaleMult = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (doIt)
        {
            doIt = false;
            DoSquash();
        }

        time = Mathf.Min(time + Time.deltaTime*speed, 1);

        float wavePoint = -amp * Mathf.Pow(-time + 1, pow) * Mathf.Sin(2*Mathf.PI*freq*time);

        if (!flipXY)
        {
            scale.y = wavePoint + 1;
            scale.x = -wavePoint*perpAmp + 1;
        }
        else
        {
            scale.y = -wavePoint*perpAmp + 1;
            scale.x = wavePoint + 1;
        }

        transform.localScale = scale * scaleMult;

        if (doPivot) 
        {
            transform.localPosition = new Vector2(0, pivot.y*(1-scale.y));
        }
        else
        {
            transform.localPosition = new Vector2(0, 0);
        }
    }

    public void DoSquash()
    {
        time = 0;
    }
    public void DoSquash(float s, float a, float f, float p)
    {
        speed = s;
        amp = a;
        freq = f;
        pow = p; 

        time = 0;
    }
    public void DoSquash(SquashData data)
    {
        speed = data.speed;
        amp = data.amplitude;
        freq = data.freqency;
        pow = data.decayPower;

        flipXY = data.flipXY;
        doPivot = data.doPivot;

        perpAmp = data.perpAmp;

        time = 0;
    }
}
[System.Serializable]
public class SquashData
{
    public string name;

    public float speed = 3;
    public float amplitude = 0.2f;
    public float freqency = 2;
    public float decayPower = 2;

    public bool flipXY;
    public bool doPivot;

    public float perpAmp = 1;

    public SquashData CloneYourself()
    {
        SquashData data = new SquashData();

        data.name = name;
        data.speed = speed;
        data.amplitude = amplitude;
        data.freqency = freqency;
        data.decayPower = decayPower;
        data.flipXY = flipXY;
        data.doPivot = doPivot;
        data.perpAmp = perpAmp;

        return data;
    }
}
