using UnityEngine;
using System.Collections;
 
[System.Serializable]
public class Game { 
 
    public static Game current;
    
    public static int highScore { 
        get { return PlayerPrefs.GetInt( "HighScore" , 0); }
        set { if( value > highScore ){ PlayerPrefs.SetInt( "HighScore" , value ); }}
    }

    public static string agentName {
        get { 
            string Name = PlayerPrefs.GetString( "AgentName" , SetAgentName() );
            PlayerPrefs.SetString( "AgentName" , Name );
            return Name;
        }
    }


    public static int lastScore { 
        get { return PlayerPrefs.GetInt( "LastScore" , 0 ); }
        set { PlayerPrefs.SetInt( "LastScore" , value );}
    }

    public static int yourScore = 0;/* { 
        get { return PlayerPrefs.GetInt( "YourScore" , 0); }
        set { PlayerPrefs.SetInt( "YourScore" , value ); }
    }*/

 
    public static string SetAgentName(){
        Random.seed = (int)System.DateTime.Now.Ticks;

        string[] first = new string[]{
          "Agenet",
          "Kernel",
          "Generollololmao",
          "Arficer",
          "Sargenet",
          "Lieutentante",
          "Mayjerk",
          "Captaint",
          "Corporollolol"
        };
        
        string[] full = new string[]{
          "Warka",
          "Flarka",
          "Flim",
          "Flam",
          "Narfy",
          "Blarp",
          "Swarfy",
          "Farbin",
          "Garblers",
          "Thumbleton",
          "Pumpkin",
          "Flarkin",
          "Bodkin", 
          "Gobular",
          "Blunk",
          "Slurf",
          "Chunswice",
          "Hurkulent",
          "Saggly",
          "Garbonzo",
          "Booboo-booboo",
          "Bluh",
          "Squanch",
          "Squeeelch",
          "Krundfisker",
          "Krullnch",
          "Borchel",
          "Gargble",
          "Flelsh",
          "Glabosk",
          "Moiboy",
          "Unchy",
          "Squeen",
          "Squarnch",
          "Snorfp",
          "Plorgorl",
          "Nomfnomf",
          "Phlongolph",
          "Blargler"
        };

        string[] title = new string[]{
          "Unicorn Slayer",
          "Turt Burgler",
          "Doge Master",
          "Grampleton",
          "truuuf",
          "Beast Dominatrix",
          "Void Traveler",
          "Abyss Starer",
          "lol wat?",
          "Sparkle Juice",
          "Universe Destroyer",
          "Entropy Ignorer",
          "Basket Weaver",
          "Flarka Flipper",
          "Blarp Wrangler",
          "Donut Dominatrix",
          "Doggo"
        };

        string[] theThird = new string[]{
          "I",
          "II",
          "III",
          "IV",
          "V",
          "VI",
          "VIII",
          "X",
          "MM",
          "XV",
          "XVX",
          "XXX"
        };

        string o = first[Random.Range(0, first.Length - 1)];
        string oo = full[Random.Range(0, full.Length - 1)];
        string ooo = title[Random.Range(0, title.Length - 1)];
        string oooo = full[Random.Range(0, full.Length - 1)];
        string ooooo = theThird[Random.Range(0, theThird.Length - 1)];


        string fullName =  o + " " + oo + " '" + ooo + "' " + oooo + " " + ooooo;
        Debug.Log(fullName);
        return fullName;

    }

    public Game () {
        //highScore = 0;
        ////lastScore = 0;
        //yourScore = 0;
        //current = this;
    }
         
}