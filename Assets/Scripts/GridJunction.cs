using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridJunction 
{
    public GridMaker GridMaker;

    public GridPoint[] GridPoints = new GridPoint[3];

    [SerializeField]
    private int x;
    public int X => x;
    [SerializeField]
    private int y;
    public int Y => y;

    [SerializeField]
    private Vector3 localPosition;
    public Vector3 LocalPosition => localPosition;
    public Vector3 WorldPosition => GridMaker.Instance.transform.TransformPoint(localPosition);

    public bool isOdd => X % 2 > 0 ? true : false;

    public GridJunction(GridMaker gridMaker , int x , int y)
    {
        GridMaker = gridMaker;
        this.x = x;
        this.y = y;

        int gridX = x / 2;

        if(isOdd)
        {
            GridPoints[0] = GridPoint.All[gridX, gridX % 2 > 0 ? y : y + 1];
            GridPoints[1] = GridPoint.All[gridX + 1, y];
            GridPoints[2] = GridPoint.All[gridX + 1, y + 1];
        } else
        {
            GridPoints[0] = GridPoint.All[gridX + 1, gridX % 2 > 0 ? y + 1 : y];
            GridPoints[1] = GridPoint.All[gridX, y];
            GridPoints[2] = GridPoint.All[gridX, y + 1];
        }

        localPosition = new Vector3((GridPoints[0].LocalPosition.x + GridPoints[1].LocalPosition.x) / 2,
            GridPoints[0].LocalPosition.y);

    }

    public void SwitchHexagonsClockwise()
    {
        Hexagon[] hexagons = new Hexagon[] { GridPoints[0].Hexagon, GridPoints[1].Hexagon, GridPoints[2].Hexagon };
        if(isOdd)
        {
            GridPoints[0].Hexagon = hexagons[2];
            GridPoints[1].Hexagon = hexagons[0];
            GridPoints[2].Hexagon = hexagons[1];
        } else
        {
            GridPoints[0].Hexagon = hexagons[1];
            GridPoints[1].Hexagon = hexagons[2];
            GridPoints[2].Hexagon = hexagons[0];
        }
    }

    public void SwitchHexagonsCounterClockwise()
    {
        Hexagon[] hexagons = new Hexagon[] { GridPoints[0].Hexagon, GridPoints[1].Hexagon, GridPoints[2].Hexagon };
        if (isOdd)
        {
            GridPoints[0].Hexagon = hexagons[1];
            GridPoints[1].Hexagon = hexagons[2];
            GridPoints[2].Hexagon = hexagons[0];
        }
        else
        {
            GridPoints[0].Hexagon = hexagons[2];
            GridPoints[1].Hexagon = hexagons[0];
            GridPoints[2].Hexagon = hexagons[1];
        }
    }
}
