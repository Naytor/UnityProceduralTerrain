using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ntWorldCreate))]
public class ntWorldEditor : Editor {

  ntWorldCreate world;
  Editor worldEditor;
  
  public override void OnInspectorGUI(){
    using (var check = new EditorGUI.ChangeCheckScope()){  
      base.OnInspectorGUI();
      if (check.changed){
	world.OnWorldSettingsUpdate();
      }
    }
    if(GUILayout.Button("Generate World")){
      world.GenerateWorld();
    }
    DrawSettingsEditor(world.worldSettings, world.OnWorldSettingsUpdate, ref world.worldSettingsFoldout, ref worldEditor);    
  }
  
  void DrawSettingsEditor(Object settings, System.Action onSettingsUpdate, ref bool foldout, ref Editor editor){    
    foldout = EditorGUILayout.InspectorTitlebar(foldout,settings);
    using (var check = new EditorGUI.ChangeCheckScope()){
      if (foldout){
	CreateCachedEditor(settings, null, ref editor);
	editor.OnInspectorGUI(); 
	if (check.changed){
	  if(onSettingsUpdate != null){
	    onSettingsUpdate();
	  }
	}
      }
    }    
  }
  
  private void OnEnable() {
    world = (ntWorldCreate)target;
  }

}
