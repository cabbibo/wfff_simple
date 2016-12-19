using UnityEngine;
using System.Collections;

public class FloorCollide : MonoBehaviour {

  public delegate void FloorHit(GameObject ring , GameObject ball);
  public event FloorHit OnFloorHit;

  // Use this for initialization
  void Start () {
  }
  
  // Update is called once per frame
  void Update () {
  
  }

  void OnCollisionEnter( Collision c ){

//    print("YES");

    if( c.collider.tag == "Ball"){
      if(OnFloorHit != null) OnFloorHit( transform.gameObject , c.collider.transform.gameObject );
    }
  }
}

