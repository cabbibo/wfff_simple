using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMaterial : MonoBehaviour {
  public Material mat;
	// Use this for initialization
	void Start () {
		Renderer renderer = GetComponent<MeshRenderer>();
	    renderer.material = mat;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
