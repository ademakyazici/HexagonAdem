using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GridPoint
{

    public static GridPoint[,] All;

    public GridMaker GridMaker;
    [SerializeField]
    private Hexagon hexagon;
    public Hexagon Hexagon
    #region Property 
    {
        get => hexagon;
        set
        {
            if (value != null)
                value.GridPoint = this;
            hexagon = value;
        }
    }
    #endregion

    
    [SerializeField]
    private int x;
    public int X => x;
    [SerializeField]
    private int y;
    public int Y => y;
    

    //Sol alt köşe (0,0).
    [SerializeField]
    private Vector3 localPosition;
    public Vector3 LocalPosition => localPosition;
    public Vector3 WorldPosition => GridMaker.Instance.transform.TransformPoint(localPosition);
    [SerializeField]
    private Vector3 localStartPosition;
    public Vector3 LocalStartPosition => localStartPosition;


    public bool isOdd => X % 2 > 0 ? true : false;


    public GridPoint(GridMaker gridmaker, int x, int y, float width, float height)
    {
        this.GridMaker = gridmaker;
        this.x = x;
        this.y = y;

        float xCorrection=0.725f;
        float yCorrection = 0.97f;
        

        if(isOdd)
        {
            //0.725
            localPosition = new Vector3(x * GridMaker.HexagonScale.x * width*xCorrection, y * GridMaker.HexagonScale.y*height* yCorrection);
            localStartPosition = new Vector3(x * GridMaker.HexagonScale.x * width* xCorrection, GridMaker.Size.y + LocalPosition.y);
        }
        else
        {
            localPosition = new Vector3(x * GridMaker.HexagonScale.x * width* xCorrection, (y * GridMaker.HexagonScale.y* height * yCorrection) - (GridMaker.HexagonScale.y* height * yCorrection / 2));
            localStartPosition = new Vector3(x * GridMaker.HexagonScale.x * width* xCorrection, GridMaker.Size.y + LocalPosition.y - (GridMaker.HexagonScale.y*height* yCorrection / 2));
        }
    }

    public static bool GetCommonNeighbor(GridPoint a, GridPoint b, GridPoint excluding, out GridPoint neighbor)
    {
        neighbor = null;

        //Eğer a ve b komşu değilse false döndür.
        if ((Mathf.Abs(a.X - b.X) != 1 && Mathf.Abs(a.Y - b.Y) > 1) ||
             (Mathf.Abs(a.Y - b.Y) != 1 && Mathf.Abs(a.X - b.X) > 1)) return false;

        GridPoint temp = null;
        if(a.Y==b.Y)
        {
            if(a.isOdd)
            {
                if (a.Y > 0)
                {
                    temp = All[a.X, a.Y - 1];
                    if(temp!=excluding)
                    {
                        neighbor = temp;
                        return true;
                    }
                }

                if(b.Y<GridMaker.Instance.Size.y-1)
                {
                    temp = All[b.X, b.Y + 1];
                    if(temp != excluding)
                    {
                        neighbor = temp;
                        return true;
                    }
                }
            }
            else
            {
                if (a.Y > GridMaker.Instance.Size.y - 1)
                {
                    temp = All[a.X, a.Y + 1];
                    if (temp != excluding)
                    {
                        neighbor = temp;
                        return true;
                    }
                }

                if (b.Y > 0)
                {
                    temp = All[b.X, b.Y - 1];
                    if (temp != excluding)
                    {
                        neighbor = temp;
                        return true;
                    }
                }
            }
        
        }
        else if(a.X==b.X)
        {
            int lowestY = a.Y < b.Y ? a.Y : b.Y;
            if(a.X> 0) 
            {
                temp = All[a.X - 1, lowestY];
                if (temp != excluding)
                {
                    neighbor = temp;
                    return true;
                }
            }

            if(b.X>GridMaker.Instance.Size.x-1)
            {
                temp = All[b.X + 1, lowestY];
                if (temp != excluding)
                {
                    neighbor = temp;
                    return true;
                }
            }
        }
        return false;
            

        
    }
    







}