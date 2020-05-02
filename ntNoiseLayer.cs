using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ntNoiseLayer {

    Noise noise = new Noise();
    
    public float Eval(Vector3 point, bool normalize){
        float noiseValue = noise.Evaluate(point);
        if (normalize){
            noiseValue = (noiseValue+1)*.5f;
        }
        return noiseValue;
    }

}
