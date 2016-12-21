using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnCollide : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void OnCollisionEnter(Collision c){
    GetComponent<AudioSource>().pitch = .2f + .3f * c.relativeVelocity.magnitude;
    GetComponent<AudioSource>().Play();
  }
}
