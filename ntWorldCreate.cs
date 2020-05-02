using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ntWorldCreate : MonoBehaviour{
  
  public bool autoUpdate = true;

  //[SerializeField, HideInInspector] MeshFilter[] meshFilters;
  [SerializeField] 
  MeshFilter[] meshFilters;
  [SerializeField] 
  GameObject[] meshFilterObjs;
  ntPanel[] ntPanels;
  
  [SerializeField, HideInInspector]
  public bool worldSettingsFoldout = true;
  
  public ntWorldSettings worldSettings;
  public ntNoiseSettings[] noiseSettings;
  
  public ntShaderSettings shaderSettings;
  
  public ntGradientToTexture gradientConvert;
  
  ntPanelShaper shaper;
  Mesh pointmesh;

  void Start(){
    if (Application.isPlaying){
      GenerateWorld();
    }
  }
  
  
  void Initialize(){
    shaper = new ntPanelShaper(noiseSettings);
    gradientConvert = new ntGradientToTexture( ref shaderSettings.colorRamp,128);
    int numPanels = worldSettings.panelsx*worldSettings.panelsz;
    
    if ( (meshFilters == null) || (meshFilters.Length != numPanels) ){
      MeshFilter[] meshFiltersOrig = meshFilters;
      GameObject[] meshObjsOrig = meshFilterObjs;
      int meshFilterLengthOrig = meshFilters.Length;
      if (meshFilterLengthOrig > numPanels){	
	for (int i = numPanels; i < meshFilterLengthOrig; i++ ){
	  DestroyImmediate(meshFilterObjs[i],true);
	}
      }      
      meshFilters = new MeshFilter[numPanels];   
      meshFilterObjs = new GameObject[numPanels];
      if (meshFilterLengthOrig < numPanels){
	for (int i = 0; i < meshFilterLengthOrig; i++ ){
	  meshFilters[i] = meshFiltersOrig[i];
	  meshFilterObjs[i] = meshObjsOrig[i];
	}
      }
      if (meshFilterLengthOrig > numPanels ){
	for (int i = 0; i < numPanels; i++ ){
	  meshFilters[i] = meshFiltersOrig[i];
	  meshFilterObjs[i] = meshObjsOrig[i];
	}
      }
      //if (gradientConvert == null){
        
      //}
    }
    
    // Apply shader settings 
    worldSettings.groundMaterial.SetVector("_heightMinMax", new Vector4(shaderSettings.heightMin, shaderSettings.heightMax));   
    //gradientConvert.gradient = shaderSettings.colorRamp;
    //Texture2D texpass = new Texture2D(256,1);
    //gradientConvert = new ntGradientToTexture(shaderSettings.colorRamp,128);
    gradientConvert.Convert(worldSettings.groundMaterial);
    //worldSettings.groundMaterial.SetTexture("_heightColorRamp", texpass);\
    
    //worldSettings.groundMaterial.SetTexture("_heightColorRamp",new Texture2D(256,1));
    
    ntPanels = new ntPanel[numPanels];
    for (int i = 0; i < numPanels; i++){
      if (meshFilters[i]==null){
	GameObject meshObj = new GameObject("panel"+i);
	meshFilterObjs[i] = meshObj;
	meshObj.transform.parent = transform;
	meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
	meshFilters[i] = meshObj.AddComponent<MeshFilter>();
	meshFilters[i].sharedMesh = new Mesh();
	meshObj.AddComponent<MeshCollider>().sharedMesh = meshFilters[i].sharedMesh;
      }
      meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = worldSettings.groundMaterial;
      
      //compute panel positions
      float[] placement = new float[5];
      placement = ComputePanelPlacement(worldSettings,i);
      
      ntPanels[i] = new ntPanel(shaper,meshFilters[i].sharedMesh,worldSettings.divisionsx,worldSettings.divisionsz,placement[0],placement[1],new Vector3(placement[2],placement[3],placement[4]));
    }
    
  }
  
  public void GenerateWorld(){
    Initialize();
    GeneratePanel();    
  }
  
  void GeneratePanel(){
    foreach (ntPanel panel in ntPanels){
      panel.CreateMesh();
    }
  }
  
  public void OnWorldSettingsUpdate(){
    if (autoUpdate == true){
      Initialize();
      GeneratePanel();
    }
  }
  
  float[] ComputePanelPlacement(ntWorldSettings worldSettingsIn,int panelID){
    float[] results = new float[5];
    results[0] = worldSettingsIn.scalex/(float)worldSettingsIn.panelsx;
    results[1] = worldSettingsIn.scalez/(float)worldSettingsIn.panelsz;
    results[2] = (( worldSettingsIn.center[0] - ( results[0] * ((float)worldSettingsIn.panelsx/2) ))+(results[0]*.5f)) + ( ( results[0] * ( (float)panelID % (float)worldSettingsIn.panelsx) ) );
    results[3] = worldSettingsIn.center[1];
    results[4] = (( worldSettingsIn.center[2] - ( results[1] * ((float)worldSettingsIn.panelsz/2) ))+(results[1]*.5f)) + ( ( results[1] * ( (float)(panelID/(worldSettingsIn.panelsx) ) % (float)worldSettingsIn.panelsz) ) );
    return results;
  }
  
}
