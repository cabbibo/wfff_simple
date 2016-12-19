using UnityEngine;
using System.Collections;

public class Snake : MonoBehaviour {

  public float speed;
  public delegate void BallEat(GameObject ring , GameObject ball);
  public event BallEat OnBallEat;

  public GameObject ball;
  public Transform ringTransform;

  public AudioClip sonar;
  public AudioClip ouch;

  public bool awake = false;

  public float justHit = 1;

  private bool playSonar = false;

  private Vector3 spinVel;

  Vector3 v1;
	// Use this for initialization
	void Start () {
	
	}

  Vector3 getPosition( Vector3 x , Vector3 y , float val ){
    return( 2.5f*x * Mathf.Sin( val * 4 ) + 2.5f* y * Mathf.Cos( val * 4) ); 
  }
	
	// Update is called once per frame
	void Update () {

    GetComponent<LineRenderer>().SetPosition( 0 , transform.position );
    if( awake == true ){

      float ojh = justHit;
      justHit -= .01f;
      if( justHit < 0 && ojh >= 0 ){
        startSonar();
      }
      v1 = transform.position - ball.transform.position;
      GetComponent<Rigidbody>().AddForce( -v1.normalized * speed );
      GetComponent<LineRenderer>().SetPosition( 1 , ball.transform.position );

      if( playSonar == true ){
        float m = v1.magnitude;
        GetComponent<AudioSource>().pitch = Mathf.Clamp( 3/m , 0 , 10);
        GetComponent<AudioSource>().volume = Mathf.Clamp( .5f/m , 0 , 1f);
      }
      GetComponent<MeshRenderer>().material.SetFloat( "_Asleep" , 0 );
      GetComponent<TrailRenderer>().material.SetFloat( "_Asleep" , 0 );
      
    }else{




      Vector3 x = ringTransform.TransformVector(Vector3.right);
      Vector3 y = ringTransform.TransformVector(Vector3.up);
      spinVel = getPosition(x,y,Time.time );
      transform.position = spinVel + ringTransform.position;
      spinVel = getPosition( x,y,Time.time + .0001f ) - spinVel;
      spinVel = spinVel.normalized;
      GetComponent<LineRenderer>().SetPosition( 1 , transform.position );
      GetComponent<MeshRenderer>().material.SetFloat( "_Asleep" , 1 );
      GetComponent<TrailRenderer>().material.SetFloat( "_Asleep" , 1);
      GetComponent<AudioSource>().volume = 0;
      

    }


	
	}

  void startSonar(){
v1 = transform.position - ball.transform.position;

      float m = v1.magnitude;
      GetComponent<AudioSource>().pitch = Mathf.Clamp( 3/m , 0 , 10);
      GetComponent<AudioSource>().volume = Mathf.Clamp( .1f/m , 0 , .2f);
    GetComponent<AudioSource>().clip = sonar;
 
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();
    playSonar = true;
  }

  public void Awaken(GameObject awakenBall){
    ball = awakenBall;
    awake = true;

    startSonar();
    

    Vector3 x = ringTransform.TransformVector(Vector3.right);
    Vector3 y = ringTransform.TransformVector(Vector3.up);

    GetComponent<Rigidbody>().velocity = spinVel;
  }

  void OnCollisionEnter( Collision c ){

    if( c.collider.tag == "Ball"){

      if( awake == true){
        if(OnBallEat != null) OnBallEat( transform.gameObject , c.collider.transform.gameObject );
      }
    } 

    if( awake == true){
      if( c.collider.tag == "Hand"){

        print("YA");
        GetComponent<AudioSource>().clip = ouch;
        GetComponent<AudioSource>().pitch = 1 / (.1f+.1f*c.relativeVelocity.magnitude);
        GetComponent<AudioSource>().volume = 1;
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().Play();
        playSonar = false;
        justHit =c.relativeVelocity.magnitude*1;
      }
    }

  }
}
