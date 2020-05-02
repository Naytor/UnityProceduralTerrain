using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ntPanelShaper{

    ntNoiseLayer noiseLayer;
    ntNoiseSettings[] noiseSettings; 
    
    public ntPanelShaper(ntNoiseSettings[] settings){
        noiseLayer = new ntNoiseLayer();
        noiseSettings = new ntNoiseSettings[settings.Length];
        for (int i=0; i<noiseSettings.Length; i++){
            noiseSettings[i] = settings[i];         
        }
    }
  
    public Vector3 CalculatePointDisplacement(Vector3 pointWorldPosition) {
    
        float compositeLayerDisplacement = 0f;
        for ( int j = 0; j < noiseSettings.Length; j++ ){
            if (noiseSettings[j].enable){ 
                float multiplyloop = noiseSettings[j].multiplier;
                float frequencyloop = noiseSettings[j].scale;
                float noiseAccumulate = 0;
                bool doNormalize = noiseSettings[j].normalizeOutput;
                
                if (noiseSettings[j].octaveInflection || noiseSettings[j].globalInflection){ doNormalize = false; }
                
                for (int i = 0; i<noiseSettings[j].octaves; i++){   
                    
                    float noisevalue = noiseLayer.Eval((pointWorldPosition+noiseSettings[j].offset)*frequencyloop,doNormalize);
                    if (noiseSettings[j].octaveInflection){
                        noisevalue = ((1-Mathf.Abs(noisevalue))*2-1);
                    }            
                    
                    noiseAccumulate += noisevalue*multiplyloop;             
                    multiplyloop *= noiseSettings[j].multiplierStep;
                    frequencyloop *= noiseSettings[j].frequency;              
                }
                    
                if (noiseSettings[j].globalInflection){
                    noiseAccumulate = Mathf.Abs(noiseAccumulate)*2-noiseSettings[j].multiplier;

                    if (noiseSettings[j].globalInflectionInverse){
                        noiseAccumulate = (noiseSettings[j].multiplier-noiseAccumulate)-noiseSettings[j].multiplier;
                    }
                }
                
                if (noiseSettings[j].inverse){
                    noiseAccumulate = (noiseSettings[j].multiplier-noiseAccumulate)-noiseSettings[j].multiplier;
                    if ( (doNormalize == true) && (noiseSettings[j].normalizeOutput == true) ){
                        noiseAccumulate += noiseSettings[j].multiplier;
                    }
                }
                
                if ( (doNormalize == false) && (noiseSettings[j].normalizeOutput == true) ){
                    noiseAccumulate =  noiseAccumulate*.5f + noiseSettings[j].multiplier;
                } 
                
                noiseAccumulate += noiseSettings[j].shift;

                // deformation past this point is applied to the worldspace height
                float finalPositionY = pointWorldPosition[1]+noiseAccumulate;
                
                if (noiseSettings[j].bands > 0){
                    float bandIncriment = (noiseSettings[j].bandsEnd-noiseSettings[j].bandsStart)/noiseSettings[j].bands;
                    float bandRingRadius = (bandIncriment*noiseSettings[j].bandWidth)/2; 
                    float bandRingStartOffset = (bandIncriment-(bandIncriment*noiseSettings[j].bandWidth))/2;
                    float bandRingEndOffset = ((bandIncriment*noiseSettings[j].bandWidth))+bandRingStartOffset;
                
                    for (int i = 0; i < noiseSettings[j].bands; i++){
                        if ( (finalPositionY < ( noiseSettings[j].bandsStart+(i*bandIncriment)+bandRingEndOffset )) && (finalPositionY > ( noiseSettings[j].bandsStart+(i*bandIncriment)+bandRingStartOffset )) ){
                            
                            float bandcenter = noiseSettings[j].bandsStart+(i*bandIncriment)+(bandIncriment*.5f);
                            float blenddifference = (bandRingRadius-Mathf.Abs(bandcenter-finalPositionY));
                            float blendvalue = 1;
                            if ( (blenddifference > 0) && (blenddifference < (bandRingRadius*noiseSettings[j].bandFalloff)) ){
                                blendvalue = blenddifference/(bandRingRadius*noiseSettings[j].bandFalloff);
                            }
                            
                            float bandposition = noiseSettings[j].bandsStart+(i*bandIncriment)+(bandIncriment*noiseSettings[j].bandBias);
                            finalPositionY = ((finalPositionY*(1-blendvalue))+(bandposition*blendvalue));
                        }    
                    }
                }
                
                if (finalPositionY < noiseSettings[j].clampMin){ finalPositionY = noiseSettings[j].clampMin; }
                if (finalPositionY > noiseSettings[j].clampMax){ finalPositionY = noiseSettings[j].clampMax; }
                compositeLayerDisplacement += finalPositionY;
            }
        }
        
        return new Vector3(pointWorldPosition[0],compositeLayerDisplacement,pointWorldPosition[2]);
    }
}
