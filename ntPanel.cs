using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ntPanel {
  
  //ntWorldSettings settings;
  ntPanelShaper shaper;
  Mesh mesh;
  int divisionsx;
  int divisionsz;
  float scalex; 
  float scalez; 
  Vector3 center;
  
  //public ntPanel(ntPanelShaper shaper, Mesh mesh, ntWorldSettings settings){
  public ntPanel(ntPanelShaper shaper, Mesh mesh, int divisionsx, int divisionsz, float scalex, float scalez, Vector3 center){
    //this.settings = settings;
    this.shaper = shaper;
    this.mesh = mesh;
    
    this.divisionsx = divisionsx;
    this.divisionsz = divisionsz;
    this.scalex = scalex;
    this.scalez = scalez;
    this.center = center;
    /*
    this.divisionsx = settings.divisionsx;
    this.divisionsz = settings.divisionsz;
    this.scalex = settings.scalex;
    this.scalez = settings.scalez;
    this.center = settings.center;
    */
  }
  
  public void CreateMesh(){
  
    //mesh = mesh;
    //mesh.name = "Mesh Patch";
    
    // initialize vertex and uv arrays
    Vector3[] meshvertices;
    Vector2[] meshuv;     
    meshvertices = new Vector3[ (divisionsx+1) * (divisionsz+1) ];
    meshuv = new Vector2[meshvertices.Length];
    
    // set vertex positions
    Vector3 startpoint = new Vector3();
    startpoint[0] = center[0]-(scalex/2);
    startpoint[2] = center[2]+(scalez/2);
    float incrimentx = scalex/(float)divisionsx;
    float incrimentz = scalez/(float)divisionsz;
    
    for (int z = 0, i= 0; z <= divisionsz; z++) {
      for (int x = 0; x <= divisionsx; x++, i++) {	 
	Vector3 pointposition = new Vector3();
	pointposition[0] = startpoint[0]+(incrimentx*(float)x);	
	pointposition[1] = center[1];
	pointposition[2] = startpoint[2]-(incrimentz*(float)z);	  	
	meshvertices[i] = shaper.CalculatePointDisplacement(pointposition);
	meshuv[i] = new Vector2( ((float)x/divisionsx) , ((float)z/divisionsz) );
      }
    }
    
    // initialize triangle array size 
    int[] meshtriangles = new int[(int)divisionsx * (int)divisionsz * 6];
    
    // set triangles
    for (int z = 0, ia = 0, ib = 0; z < divisionsz; z++, ia++) {
      for (int x = 0; x < divisionsx; x++, ia++, ib += 6) {
	meshtriangles[ib] = ia;
	meshtriangles[ib+1] = ia+1;	  
	meshtriangles[ib+2] = ia+divisionsx+1;
	meshtriangles[ib+3] = ia+1;	  
	meshtriangles[ib+4] = ia+divisionsx+2;	  
	meshtriangles[ib+5] = ia+divisionsx+1;	  
      }
    }

    mesh.Clear();
    mesh.vertices = meshvertices;
    mesh.uv = meshuv;    
    mesh.triangles = meshtriangles;      
    mesh.RecalculateNormals();
  
  }
}
