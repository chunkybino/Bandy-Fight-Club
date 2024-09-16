using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteInEditMode]
public class PositionConstraintControl : MonoBehaviour
{
    public PositionConstraint posCon;

    public bool weightBasedOffSourceDistance;
    public Transform closeTransformCompare;

    void Update()
    {
        if (weightBasedOffSourceDistance)
        {
            List<ConstraintSource> sources = new List<ConstraintSource>();
            posCon.GetSources(sources);
            float[] sourceDistances = new float[sources.Count] ;
            float smallestDistance = 5;
            for (int i = 0; i < sources.Count; i++) //distances
            {
                sourceDistances[i] = Vector3.Distance(sources[i].sourceTransform.position,closeTransformCompare.position);
                if (sourceDistances[i] < smallestDistance) smallestDistance = sourceDistances[i];
            }
            for (int i = 0; i < sources.Count; i++) //set weight based off distance
            {
                float weight = sources[i].weight;
                weight = smallestDistance / sourceDistances[i];
                ConstraintSource source = new ConstraintSource() {sourceTransform = sources[i].sourceTransform, weight = weight};
                sources[i] = source;
                //source.sourceTransform = sources[i].sourceTransform;
                //source.weight = weight;
                //print(weight);// = sourceDistances[i] / highestDistance;
                //sources[i].weight = sourceDistances[i] / highestDistance;
            }
            posCon.SetSources(sources);
        }
    }
}
