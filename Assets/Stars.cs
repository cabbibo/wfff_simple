using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour {

  public GameObject ball;
  public Material material;
  public Material lineMat;
  public Vector3 scale;

  public int numberParticles;

	// Use this for initialization
	void Start () {
		
	}

  void OnEnable(){
    Camera.onPostRender += Render;
  }

  void OnDisable(){
    Camera.onPostRender -= Render;
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void Render(Camera c){

    if( material != null ){

      //print("syhs");

      /*material.SetPass(0);
      material.SetVector( "_Ball" , ball.transform.position  );
      material.SetVector( "_Scale" , scale  );

      Graphics.DrawProcedural(MeshTopology.Triangles, numberParticles * 6 );*/


      lineMat.SetPass(0);
      lineMat.SetVector( "_Ball" , ball.transform.position  );
      lineMat.SetVector( "_Scale" , scale  );

      Graphics.DrawProcedural(MeshTopology.Lines, numberParticles * 2 );
    }

  }
}
