using UnityEngine;
using System.Collections;

public class resetOnPositionOnTrigger : MonoBehaviour {

  private Vector3 ogPos;
	// Use this for initialization
	void Start () {

    ogPos = transform.position;


    EventManager.OnTriggerDown += OnTriggerDown;


    //posArray = new Vector3[10];
  }
	
	
  void OnTriggerDown( GameObject c ){

    transform.position = c.transform.position;
    transform.position += Vector3.up * .3f;
    GetComponent<Rigidbody>().velocity = Vector3.zero;
  }
	
	// Update is called once per frame
	void Update () {
	
	}
}
