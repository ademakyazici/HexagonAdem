using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Hexagon
{
    public static List<Bomb> All = new List<Bomb>();

    public TextMesh TextMesh;
    public string Text
    #region Property
    {
        get => TextMesh.text;
        set => TextMesh.text = value;
    }
    #endregion

    [System.NonSerialized]
    public int CreatedAtMove;
    [System.NonSerialized]
    public int Countdown;
    public int RemainingMoves => Countdown - (Menu.Instance.NumOfMoves - CreatedAtMove);

    private static bool exploded;
    public static bool Exploded
    #region Property 
    {
        get
        {
            bool returnVal = exploded;
            exploded = false;
            return returnVal;
        }
        set => exploded = value;
    }
    #endregion

    public static Bomb CreateNew(GridPoint gridPoint)
    {
        Bomb bomb = Hexagon.CreateNewSprite().AddComponent<Bomb>();
        bomb.gameObject.name = "bomb";
        bomb.transform.localScale = new Vector3(GridMaker.Instance.HexagonScale.x, GridMaker.Instance.HexagonScale.x);
        bomb.gameObject.SetActive(true);
        Bomb.All.Add(bomb);

        bomb.SpriteRenderer.sprite = GridMaker.Instance.BombSprite;
        bomb.SpriteRenderer.sortingOrder = 1;

        GameObject temp = new GameObject("textMesh");
        temp.transform.parent = bomb.transform;
        temp.transform.parent = bomb.transform;
        bomb.TextMesh = temp.AddComponent<TextMesh>();
        bomb.TextMesh.alignment = TextAlignment.Center;
        bomb.TextMesh.anchor = TextAnchor.MiddleCenter;
        bomb.TextMesh.color = Color.white;
        bomb.TextMesh.characterSize = 0.5f;
        bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().sortingOrder = 9;

        gridPoint.Hexagon = bomb;
        bomb.Color = GridMaker.Instance.HexagonColors[Random.Range(0, GridMaker.Instance.HexagonColors.Length)];
        bomb.TimeActivated = Time.time;
        bomb.transform.localPosition = bomb.GridPosStart;
        bomb.CreatedAtMove = Menu.Instance.NumOfMoves;
        bomb.Countdown = Random.Range(7, 10);
        bomb.Tick();

        return bomb;

    }

    public static void TickAllBombs()
    {
        foreach(Bomb bomb in Bomb.All)
        {
            if(bomb==null)
            {
                Bomb.All.Remove(bomb);
                continue;
            }

            bomb.Tick();
        }
    }

    private void Tick()
    {
        TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = true;
        TextMesh.text = RemainingMoves.ToString();

    }

    public static void CheckFuses()
    {
        foreach(Bomb bomb in Bomb.All)
        {
            if (bomb==null)
            {
                Bomb.All.Remove(bomb);
                continue;
            }

            if(bomb.RemainingMoves<=0)
            {
                Menu.Instance.Restart();
                Bomb.Exploded = true;
                return;
            }
        }
    }


}
