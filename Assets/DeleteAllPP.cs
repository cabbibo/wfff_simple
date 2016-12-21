using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAllPP : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerPrefs.DeleteAll();
	}
	
	// Update is called once per frame
	void Destroy () {
		
	}
}
