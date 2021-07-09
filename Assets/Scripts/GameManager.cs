using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Player[] myPlayers = null;

    public Player myPlayerPrefab = null;

    private Player myCurrentPlayer = null;

    private int myDice1 = -1;
    private int myDice2 = -1;
    private int myDice3 = -1;

    private int myCurrentPlayerIndex = -1;

    private bool myIsInGame = false;

    public Text myPlayer1ScoreText = null;
    public Text myPlayer2ScoreText = null;
    public Text myPlayer3ScoreText = null;

    public Text myDice1Text = null;
    public Text myDice2Text = null;
    public Text myDice3Text = null;

    public Text myResultText = null;

    private bool myCanRollDice = true;

    private const int myMaxScore = 343;

    private void Start()
    {
        myPlayers = new Player[3];
        for(int i = 0; i < myPlayers.Length; i++)
        {
            myPlayers[i] = Instantiate(myPlayerPrefab);
        }

        myCurrentPlayer = myPlayers[0];

        myCurrentPlayer.myIsMyTurn = true;

        myIsInGame = true;

        StartCoroutine("LoopGame");
    }

    private void Update()
    {
        string grelottine = "";
        if (myPlayers[0].myHasGrelottine)
            grelottine = "(G)";
        else
            grelottine = "";

        if (myCurrentPlayerIndex == 0)
        {
            myPlayer1ScoreText.text = "<color=red>Player 1" + grelottine + " : " + myPlayers[0].myScore + " </color>";
        }
        else
        {
            myPlayer1ScoreText.text = "Player 1" + grelottine + " : " + myPlayers[0].myScore;
        }

        if (myPlayers[1].myHasGrelottine)
            grelottine = "(G)";
        else
            grelottine = "";

        if (myCurrentPlayerIndex == 1)
        {
            myPlayer2ScoreText.text = "<color=red>Player 2" + grelottine + " : " + myPlayers[1].myScore + "</color>";
        }
        else
        {
            myPlayer2ScoreText.text = "Player 2" + grelottine + " : " + myPlayers[1].myScore;
        }

        if (myPlayers[2].myHasGrelottine)
            grelottine = "(G)";
        else
            grelottine = "";

        if (myCurrentPlayerIndex == 2)
        {
            myPlayer3ScoreText.text = "<color=red>Player 3" + grelottine + " : " + myPlayers[2].myScore + "</color>";
        }
        else
        {
            myPlayer3ScoreText.text = "Player 3" + grelottine + " : " + myPlayers[2].myScore;
        }
    }

    private IEnumerator LoopGame()
    {
        while(myIsInGame)
        {
            ChangePlayer();

            myCanRollDice = true;

            RollDices(Random.Range(0, 1.0f));

            if (myCanRollDice)
            {
                //DicesToString();

                AnalyzeDices();
            }

            CheckScore();

            yield return new WaitForSeconds(0.5f);

            yield return null;
        }
    }

    private void RollDices(float aChangeToMiss)
    {
        float rand = Random.Range(0, 1.0f);
        if(rand < aChangeToMiss)
        {
            Bevue();
            myCanRollDice = false;
            myDice1Text.text = "X";
            myDice2Text.text = "X";
            myDice3Text.text = "X";
            return;
        }

        myDice1 = Random.Range(1, 10);
        myDice2 = Random.Range(1, 10);
        myDice3 = Random.Range(1, 10);

        myDice1Text.text = myDice1.ToString();
        myDice2Text.text = myDice2.ToString();
        myDice3Text.text = myDice3.ToString();
    }

    private void ChangePlayer()
    {
        myCurrentPlayerIndex++;
        if(myCurrentPlayerIndex >= myPlayers.Length)
        {
            myCurrentPlayerIndex = 0;
        }

        myCurrentPlayer = myPlayers[myCurrentPlayerIndex];
    }

    private void DicesToString()
    {
        Debug.Log("Dice 1 :" + myDice1);
        Debug.Log("Dice 2 :" + myDice2);
        Debug.Log("Dice 3 :" + myDice3);
    }

    private void AnalyzeDices()
    {
        //Debug.Log("Resultat :");

        int[] order = new int[]{ myDice1, myDice2, myDice3};

        bool change = true;
        
        while(change)
        {
            change = false;
            for (int i = 0; i < 2; i++)
            {
                if(order[i] > order[i + 1])
                {
                    int temp = order[i];
                    order[i] = order[i + 1];
                    order[i + 1] = temp;
                    change = true;
                }
            }
        }

        int smaller = order[0];
        int medium = order[1];
        int larger = order[2];

        if (smaller == medium && medium == larger)
        {
            //Debug.Log("Cul de Chouette");
            myResultText.text = "Cul de Chouette";
            myCurrentPlayer.myScore += 40 + (10 * smaller);
        }
        else if (smaller == medium || medium == larger || smaller == larger)
        {
            if(smaller == medium && smaller + medium == larger)
            {
                //Debug.Log("Chouette Velute");
                myResultText.text = "Chouette Velute";
                myCurrentPlayer.myScore += (larger * larger) * 2;
            }
            else
            {
                //Debug.Log("Chouette");
                myResultText.text = "Chouette";
                myCurrentPlayer.myScore += smaller * smaller;
            }
        }
        else if (smaller + medium == larger)
        {
            //Debug.Log("Velute");
            myResultText.text = "Velute";
            myCurrentPlayer.myScore += (larger * larger) * 2;
        }
        else if (smaller + 1 == medium && medium + 1 == larger)
        {
            //Debug.Log("Suite");
            myResultText.text = "Suite";
            myCurrentPlayer.myScore -= 10;
        }
        else
        {
            //Debug.Log("Néant");
            myResultText.text = "Néant";
            myCurrentPlayer.myHasGrelottine = true;
        }
    }

    private void Bevue()
    {
        //Debug.Log("Bévue");
        myResultText.text = "Bévue";
        if (myCurrentPlayer.myScore >= 10)
        {
            myCurrentPlayer.myScore -= 10;
        }
        else
        {
            myCurrentPlayer.myScore = 0;
        }
    }

    private void CheckScore()
    {
        if(myPlayers[0].myScore >= myMaxScore)
        {
            myIsInGame = false;
            myResultText.text = "Partie Finie";
        }
        else if (myPlayers[1].myScore >= myMaxScore)
        {
            myIsInGame = false;
            myResultText.text = "Partie Finie";
        }
        else if (myPlayers[2].myScore >= myMaxScore)
        {
            myIsInGame = false;
            myResultText.text = "Partie Finie";
        }
    }
}
