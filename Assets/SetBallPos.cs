using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBallPos : MonoBehaviour {

  public GameObject ball;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    GetComponent<Renderer>().material.SetVector("_Ball", ball.transform.position );
		
	}
}
