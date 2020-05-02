using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ntWorldSettings : ScriptableObject {
  
  public int panelsx;
  public int panelsz;
  [Range(0,255)]
  public int divisionsx;
  [Range(0,255)]
  public int divisionsz;
  public float scalex;
  public float scalez; 
  public Vector3 center;
  
  //public Color groundColor;
  public Material groundMaterial;
  
}
