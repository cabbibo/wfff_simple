using UnityEngine;
using System.Collections;

public class SetScale : MonoBehaviour {

  public GameObject hand1;
  public GameObject hand2;
  public GameObject head;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    GetComponent<Renderer>().material.SetVector( "_Scale", transform.localScale );
    GetComponent<Renderer>().material.SetVector( "_MyHandL", hand1.transform.position );
    GetComponent<Renderer>().material.SetVector( "_MyHandR", hand2.transform.position );
    GetComponent<Renderer>().material.SetVector( "_MyHead", head.transform.position );
	}

}
