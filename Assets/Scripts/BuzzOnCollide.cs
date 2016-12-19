using UnityEngine;
using System.Collections;

public class BuzzOnCollide : MonoBehaviour {

  public GameObject controller;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnCollisionEnter(Collision c){
//    print( c.impulse.magnitude );

     if( c.collider.tag == "Ball"){
    SteamVR_TrackedObject tObj =
    controller.GetComponent<SteamVR_TrackedObject>(); if( (int)tObj.index !=
    -1 ){
      
      var device = SteamVR_Controller.Input((int)tObj.index);
      var v = 3999; ///c.impulse.magnitude * 200;
      device.TriggerHapticPulse((ushort)v);
    }
    GetComponent<AudioSource>().pitch = (2 - c.relativeVelocity.magnitude / 5);
    GetComponent<AudioSource>().Play();
  }


  }

  void OnCollisionStay(Collision c){
if( c.collider.tag == "Ball"){
    SteamVR_TrackedObject tObj = controller.GetComponent<SteamVR_TrackedObject>();

    //print((int)tObj.index );

    if( (int)tObj.index != -1 ){
      var device = SteamVR_Controller.Input((int)tObj.index);
      var v = 50; ///c.impulse.magnitude * 200;
      device.TriggerHapticPulse((ushort)v);
    } 

  }
  }
}
