using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBallAudioStuff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    GetComponent<AudioSource>().volume = Mathf.Min( .5f, GetComponent<Rigidbody>().velocity.magnitude * .2f + .1f) / (1+.3f* transform.position.magnitude);
    GetComponent<AudioSource>().pitch = GetComponent<Rigidbody>().velocity.magnitude + .3f;

    if( GetComponent<Ball>().frozen == true ){
      GetComponent<AudioSource>().volume = 0;
    }
		
	}
}
