using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    [SerializeField] ShakeWaves shakeWaves;
    [SerializeField] float speedMult = 1;
    [SerializeField] float ampMult = 1;
    [SerializeField] float shakeDecayFactor = 3; //square the timeAmp by some number so we get acurved decay or smthn

    [SerializeField] float currentShakeTime;
    [SerializeField] float totalShakeTime = 1;

    [SerializeField] ShakeData currentShakeData;
    [SerializeField] Vector2 vecAmpMult = new Vector2(1,0.5f);
    [SerializeField] Vector2 vecFreqMult = new Vector2(1,1.2f);
    [SerializeField] Vector2 shakeDirection;
    [SerializeField] Vector2 perpendicularAmpVarianceRange = new Vector2(0.4f, 0.65f);
    [SerializeField] Vector2 perpendicularFreqVarianceRange = new Vector2(1.05f, 1.3f);

    public bool doShakeOrSmthn;
    public float doShakeStr = 1;

    public bool doUnscaledTime;
    float deltaTime {get{if (doUnscaledTime){return Time.unscaledDeltaTime;}else{return Time.deltaTime;}}}

    void Update()
    {
        //if (GameManager.gamePaused) return;

        if (doShakeOrSmthn)
        {
            doShakeOrSmthn = false;
            DoShake(doShakeStr);
        }
        
        currentShakeTime += deltaTime;

        float timeAmp = 0; //amplitude base don how much the shake has decayd;
        float susTime = currentShakeData.sustainTime*currentShakeData.time; //sustain time is a percentage, so make it not a percentage for the math
        timeAmp = Mathf.Pow(Mathf.Lerp(1,0, (currentShakeTime-susTime)/(currentShakeData.time-susTime)), shakeDecayFactor); //square it, so we get a curved decay line or something

        Vector2 v = new Vector2(0,0);
        foreach (ShakeWaves.WaveData w in shakeWaves.shakeWaves)
        {
            if (w.freq == 0) continue;
            Vector2 f = currentShakeTime*currentShakeData.freq*speedMult*2*Mathf.PI*w.freq * vecFreqMult;
            v += new Vector2(Mathf.Sin(f.x), Mathf.Sin(f.y)) * w.amp;
        }

        float totalAmp = 0;
        foreach (ShakeWaves.WaveData w in shakeWaves.shakeWaves) {totalAmp += w.amp;}

        if (totalAmp != 0) {v = v/totalAmp;}
        else {v = new Vector2(0,0);}

        v *= vecAmpMult * currentShakeData.amp * timeAmp * ampMult;

        transform.localPosition = UFunc.RotateVecByVec(v, shakeDirection);
    }

    public void DoShake(float str)
    {
        DoShake(str, new Vector2(Random.Range(-1,1), Random.Range(-1,1)).normalized); //Random.Range(0f,360f));
    }
    public void DoShake(float str, Vector2 direction)
    {
        str = Mathf.Pow(str, 0.75f)*0.5f;
        ShakeData data = new ShakeData() {
            amp = str * 0.8f,
            time = Mathf.Pow(str, 1/3f)*0.5f,
            sustainTime = 0.2f,
            freq = Mathf.Lerp(2f,1.5f, Mathf.Pow(str, 0.5f)),
            dontDoVerticalShake = false
        };

        DoShake(data, direction);
    }

    public void DoShake(ShakeData data, Vector2 direction)
    {
        currentShakeTime = 0;
        currentShakeData = data;

        shakeDirection = direction;
        if (!data.dontDoVerticalShake)
        {
            vecAmpMult = new Vector2(1, Random.Range(perpendicularAmpVarianceRange.x, perpendicularAmpVarianceRange.y));
            vecFreqMult = new Vector2(1, Random.Range(perpendicularFreqVarianceRange.x, perpendicularFreqVarianceRange.y));
        }
        else
        {
            vecAmpMult = new Vector2(1, 0);
            vecFreqMult = new Vector2(1, 0);
        }
    }

    [System.Serializable]
    public struct ShakeData
    {
        public float time;
        public float sustainTime;
        public float amp;
        public float freq;
        public bool dontDoVerticalShake; //onyl shake back and forth in the direciton given
    }
}

[CreateAssetMenu(fileName = "ShakeWaves", menuName = "ScriptableObjects/ShaveWaves")]
public class ShakeWaves : ScriptableObject
{
    public WaveData[] shakeWaves;

    [System.Serializable]
    public struct WaveData
    {
        public float freq;
        public float amp;
    }
}
