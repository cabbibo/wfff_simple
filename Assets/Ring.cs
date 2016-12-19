using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ring : MonoBehaviour {

  public List<GameObject> snakes = new List<GameObject>();

  public AudioClip yay;
  public AudioClip boo;
  public AudioClip startClip;

  public GameObject ball;
  public GameObject gate;
  public GameObject snakePrefab;
  public GameObject floor;

  public GameObject HighScore;
  public GameObject YourScore;
  public GameObject LastScore;

  public GameObject currentSnake;

  public int currentScore = 0;
	// Use this for initialization
	void Start () {

    //SaveLoad.Load();

    if( Game.current == null){
      Game.current = new Game();
    }


    gate.GetComponent<BallPassThrough>().OnBallPass += OnBallPass;
    floor.GetComponent<FloorCollide>().OnFloorHit += OnFloorHit;
    ball.GetComponent<Ball>().OnBallStart += OnBallStart;

    UpdateScores();
    
	
	}
	
	// Update is called once per frame
	void Update () {

    if( ball.GetComponent<Ball>().frozen == false ){

      float m = GetComponent<AudioSource>().volume;
      GetComponent<AudioSource>().volume = Mathf.Max((m - .002f),0);
      ball.GetComponent<TrailRenderer>().enabled = true;

    }else{
      
    }

	
	}

  GameObject NewSnake(){
    
    
    
    transform.position = Random.insideUnitSphere + Vector3.up * 1.5f;
    transform.rotation = Random.rotation;


    Vector3 position = new Vector3(0,.5f,0);
    position = transform.TransformPoint( position );

    GameObject snake = (GameObject) Instantiate(snakePrefab, position, Quaternion.identity);

    snake.GetComponent<Snake>().OnBallEat += OnBallEat;
    snake.GetComponent<Snake>().ringTransform = transform;
    snake.GetComponent<Snake>().speed = .1f + (float)currentScore / 20;

    snakes.Add( snake );



    return snake;


  }

  void OnBallPass( GameObject ring , GameObject ball ){

    currentScore += 1;
    GetComponent<AudioSource>().clip = yay;
    GetComponent<AudioSource>().volume = .5f;
    GetComponent<AudioSource>().Play();
    UpdateScores();

    currentSnake.GetComponent<Snake>().Awaken(ball);
    currentSnake = NewSnake();

  }

  void OnFloorHit( GameObject floor , GameObject ball ){
    restart();
  }

  void OnBallEat(GameObject snake , GameObject ball){
    restart();
  }

  void UpdateScores(){

    Game.yourScore = currentScore;
    if( currentScore > Game.highScore ){ Game.highScore = currentScore; }


    HighScore.GetComponent<TextMesh>().text = "High Score: " + Game.highScore.ToString();
    YourScore.GetComponent<TextMesh>().text = "Your Score: " + Game.yourScore.ToString();
    LastScore.GetComponent<TextMesh>().text = "Last Score: " + Game.lastScore.ToString();

  }

  void restart(){
    
    foreach( GameObject snake in snakes ){
      Destroy(snake);
    }
    
    currentScore = 0;
    ball.GetComponent<TrailRenderer>().enabled = false;

    GetComponent<AudioSource>().clip = boo;
    GetComponent<AudioSource>().volume = .5f;
    GetComponent<AudioSource>().Play();

    ball.GetComponent<Ball>().restart();
    Game.lastScore = Game.yourScore;
    UpdateScores();

    //SaveLoad.Save();
  }

  void OnBallStart( GameObject g){
    currentSnake = NewSnake();
    ball.GetComponent<TrailRenderer>().enabled = true;

    GetComponent<AudioSource>().clip = startClip;
    GetComponent<AudioSource>().volume = .5f;
    GetComponent<AudioSource>().Play();
  }

}
