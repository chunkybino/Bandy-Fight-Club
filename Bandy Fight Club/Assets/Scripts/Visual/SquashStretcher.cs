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
        scale.y = wavePoint + 1;
        scale.x = -wavePoint*perpAmp + 1;
        //scaleMult.y = amp*Mathf.Pow(2*time - 1, pow) + 1 - amp;

        transform.localScale = scale * scaleMult;
        transform.localPosition = new Vector2(0, pivot.y*(1-scale.y));
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

        time = 0;
    }
}
[System.Serializable]
public class SquashData
{
    public string name;

    public float speed;
    public float amplitude;
    public float freqency;
    public float decayPower;
}
