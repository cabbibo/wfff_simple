using UnityEngine;
using System.Collections;

public class PullToController : MonoBehaviour {


  public GameObject controller;
  public GameObject location;

  private Vector3 v1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

    v1 = transform.position - location.transform.position;
    v1 = v1.normalized * controller.GetComponent<controllerInfo>().triggerVal;

    GetComponent<Rigidbody>().AddForce( -v1  * 2);
	
	}
}
