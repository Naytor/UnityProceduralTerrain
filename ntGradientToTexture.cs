using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ntGradientToTexture{
    
    Gradient gradient;
    Texture2D textureOut; 
    Color[] colours;
    int resolution;
    
    
    public ntGradientToTexture( ref Gradient gradientIn, int resolutionIn){
        resolution = resolutionIn;
        gradient = gradientIn; 
        if (textureOut == null){
            textureOut = new Texture2D(resolution,1);
            colours = new Color[resolution];
        }
    }
    
    public void Convert(Material material){
        
        for (int i = 0; i < resolution; i++){
            colours[i] = gradient.Evaluate((float)i/resolution);        
        }
        textureOut.SetPixels(colours);
        textureOut.Apply();
        material.SetTexture("_heightColorRamp", textureOut);
        //return textureOut;
        //return new Texture2D(256,1);
    }
    
}
