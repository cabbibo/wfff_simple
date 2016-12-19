using UnityEngine;
using System.Collections;
 
[System.Serializable]
public class Game { 
 
    public static Game current;
    public bool finishedTutorial;
    public static float highScore;
    public static float lastScore;
    public static float yourScore;
 
    public Game () {
        finishedTutorial = false;
        highScore = 0;
        lastScore = 0;
        yourScore = 0;
    }
         
}