using UnityEngine;
using System.Collections;

public class BallPassThrough : MonoBehaviour {

  public delegate void BallPass(GameObject ring , GameObject ball);
  public event BallPass OnBallPass;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter( Collider c ){

    if( c.tag == "Ball"){

      Vector3 dif = c.transform.position - transform.position;
      dif = transform.InverseTransformVector( dif );
      if( dif.magnitude < .5f ){
        if(OnBallPass != null) OnBallPass( transform.gameObject , c.transform.gameObject );
      }
    }
  }
}
