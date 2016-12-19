using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMorePoints : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

   GetComponent<AudioSource>().volume =  1 / (1 + 2 * (float)Game.yourScore);
		
	}
}
