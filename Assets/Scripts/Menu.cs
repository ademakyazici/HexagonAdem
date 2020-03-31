using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu Instance;

    public Text TextScore;
    public Text TextNumOfMoves;

    private int score = 0;
    public int Score
    #region Property 
    {
        get => score;
        set => TextScore.text = (score = value).ToString();
    }
    #endregion

    private int numOfMoves = 0;
    public int NumOfMoves
    #region Property
    {
        get => numOfMoves;
        set 
            {
            TextNumOfMoves.text = (numOfMoves = value).ToString();
            Bomb.TickAllBombs();
            }
        
    }
    #endregion

    private void Awake()
    {
        Menu.Instance = this;
    }

    public void Restart()
    {
        foreach (Bomb bomb in Bomb.All)
            Destroy(bomb.gameObject);
        Bomb.All.Clear();
        Hexagon.Unused.Clear();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
