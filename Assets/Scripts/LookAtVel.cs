using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtVel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    transform.LookAt( transform.position + GetComponent<Rigidbody>().velocity.normalized );
		GetComponent<Rigidbody>().rotation = transform.rotation;
	}
}
