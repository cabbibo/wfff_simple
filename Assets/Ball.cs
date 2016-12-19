using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

  public bool frozen = true;

  public delegate void BallStart(GameObject ball);
  public event BallStart OnBallStart;

  float frozenTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    if( frozen == true ){
      frozenTimer -= .1f;
      transform.position = Vector3.up;
    }
	
	}

  public void restart(){
    frozen = true;
    frozenTimer = 1;
    transform.position = Vector3.up;
  }

  void OnCollisionEnter(Collision c){
    if( frozen == true && frozenTimer < 0){
      frozen = false;
      if(OnBallStart != null) OnBallStart( transform.gameObject );
      GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
  }
}
