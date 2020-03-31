using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    public static GridMaker Instance;

    [Header("Grid Settings")]
    public Vector2Int Size = new Vector2Int(8, 9);
    public Color[] HexagonColors = new Color[5] { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };
    public Vector3 HexagonScale = new Vector3(1, 1, 1);
    public int BombAppearanceScore = 1000;
    public float HexagonActivationInterval = 0.016666f;
    public bool StartByCheckingExplosions = false;

    [Header("Sprite Settings")]
    public Sprite HexagonSprite;
    public Sprite BombSprite;

    public int HexagonVariety => HexagonColors.Length;

    [HideInInspector]
    public GridPoint[] GridPoints;
    [HideInInspector]
    public Hexagon[] Hexagons;
    [System.NonSerialized]
    public GridJunction[,] GridJunctions;
    [System.NonSerialized]
    public Selection Selection;
    [System.NonSerialized]
    private Vector3 LastClickPos;    
    [System.NonSerialized]
    public bool ExplosionOccurred = false;
    [System.NonSerialized]
    public bool GameReady = false;
    [System.NonSerialized]
    public int BombCounter;

    [System.NonSerialized]
    public float width;
    [System.NonSerialized]
    public float height;



    public void RemoveGrid()
    {
        
        if(Hexagons!=null)
        {
            for (int i = 0; i < Hexagons.Length; i++)
            {
                if (Hexagons[i] != null) DestroyImmediate(Hexagons[i].gameObject);
            }
            Hexagons = null;
            
        }
        GridPoints = null;
        
    }

    public void GenerateGrid()
    {
        width = HexagonSprite.bounds.size.x;
        height = HexagonSprite.bounds.size.y;      
        //Eğer varsa önceki Grid'i sil
        RemoveGrid();
#if UNITY_EDITOR
        if(!Application.isPlaying)
        {
            GridMaker.Instance = this;
        }
#endif

        //Grid ölçülerine göre Grid'i yerleştir.
        transform.position = new Vector3((-((Size.x - 1) * HexagonScale.x*width * 0.725f) / 2), (-((Size.y) * HexagonScale.y*height) / 2));

        //Hexagon dizisini oluşturma
        Hexagons = new Hexagon[Size.x * Size.y];

        //Hexagon Object Pool'unu doldurma
        for (int i = 0; i < Hexagons.Length; i++)
            Hexagon.CreateNew(i);

        //GridPoint Dizisi oluşturma
        GridPoints = new GridPoint[Size.x* Size.y];

        //GridPointleri Grid ölçüsüne göre doldurma
       
        for(int x = 0; x<Size.x;x++)
        {
            for(int y = 0;y<Size.y;y++)
            {
                GridPoints[x + (y * Size.x)] = new GridPoint(this, x, y,width,height);
                if (!Application.isPlaying)
                    Hexagons[x + (y * Size.x)].Activate(GridPoints[x + (y * Size.x)], true);
                
            }
        }       
        

    }

    private void Awake()
    {
        GridMaker.Instance = this;
        
    }

    private void Start()
    {
        //Kolay erişilebilirlik için iki boyutlu Gridpoint Dizisi hazırlama.
        GridPoint.All = new GridPoint[Size.x, Size.y];
        for (int i = 0; i < GridPoints.Length; i++)
            GridPoint.All[GridPoints[i].X, GridPoints[i].Y] = GridPoints[i];

        //Gridjunctionları initialize et.
        GridJunctions = new GridJunction[(Size.x - 1) * 2, (Size.y - 1)];
        for(int x =0; x<GridJunctions.GetLength(0);x++)
            for (int y = 0; y < GridJunctions.GetLength(1); y++)
            {
                GridJunctions[x, y] = new GridJunction(this, x, y);
            }

        //Bütün Hexagonları devre dışı bırak ama GridPointlerini tut.
        for(int i=0; i < Hexagons.Length;i++)
        {
            Hexagons[i].Deactivate(true);
            Hexagons[i].ActivateInSeconds(HexagonActivationInterval * (i + 1));
        }

        if (StartByCheckingExplosions) ExplosionOccurred = true;
    }

    private void Update()
    {

        //Oyun hazır olmadan oyuncu müdahalesini engelleme.
        if(!GameReady)
        {
            bool allActivated = true;
            for (int i = 0; i < Hexagons.Length; i++)
            {
                if(Hexagons[i].gameObject.activeInHierarchy && !Hexagons[i].Activated)
                {
                    allActivated = false;
                    break;
                }
            }

            foreach(Bomb bomb in Bomb.All)
            {
                if (bomb == null) continue;

                if(bomb.gameObject.activeInHierarchy && !bomb.Activated)
                {
                    allActivated = false;
                    break;
                }
            }         

            if (allActivated)
            {
                //Eğer patlama olmuşsa oyunu hemen aktifleştirme. Tekrar eden patlamaları kontrol et.
                if (ExplosionOccurred) ExplosionOccurred = CheckForExplosion();
                else GameReady = true;
            }
            return;

        }
        Bomb.CheckFuses();
        InputCommands();

    }

   

    public bool CheckForExplosion(GridJunction gridJunction=null)
    {
        int xStart = 0;
        int yStart = 0;
        int xLength = GridJunctions.GetLength(0);
        int yLength = GridJunctions.GetLength(1);

        for(int y=yStart; y<yLength;y++)
        {
            for (int x = xStart; x < xLength; x++)
            {                             
               
                if (GridJunctions[x, y].GridPoints[0].Hexagon.ColorIndex == GridJunctions[x, y].GridPoints[1].Hexagon.ColorIndex
                && GridJunctions[x, y].GridPoints[0].Hexagon.ColorIndex == GridJunctions[x, y].GridPoints[2].Hexagon.ColorIndex)
                {
                    GameReady = false;
                    int colorIndex = GridJunctions[x, y].GridPoints[0].Hexagon.ColorIndex;
                    Debug.Log("Patlama gerçekleşti : @ " + x + "," + y);
                    int[] numRemoved = new int[GridPoint.All.GetLength(0)];
                    int[] lastRemoved = new int[GridPoint.All.GetLength(0)];

                    for(int i= 0; i <GridJunctions[x,y].GridPoints.Length;i++)
                    {
                        numRemoved[GridJunctions[x, y].GridPoints[i].X]++;            
                        lastRemoved[GridJunctions[x, y].GridPoints[i].X] = GridJunctions[x, y].GridPoints[i].Y;
                        GridJunctions[x, y].GridPoints[i].Hexagon.Deactivate();
                    }

                    //Komşu noktaların kontrolü
                    GridPoint neighbor;
                    GridPoint gridPointA = GridJunctions[x, y].GridPoints[0];
                    GridPoint gridPointB = GridJunctions[x, y].GridPoints[1];
                    GridPoint gridPointC = GridJunctions[x, y].GridPoints[2];

                    if(GridPoint.GetCommonNeighbor(gridPointA,gridPointB,gridPointC,out neighbor))
                    {
                        if (neighbor!=null && neighbor.Hexagon!=null && neighbor.Hexagon.Activated 
                            && neighbor.Hexagon.ColorIndex == colorIndex)
                        {
                            numRemoved[neighbor.X]++;
                            lastRemoved[neighbor.X] = neighbor.Y > lastRemoved[neighbor.X] ? neighbor.Y : lastRemoved[neighbor.X];
                            neighbor.Hexagon.Deactivate();
                        }
                    }

                    gridPointA = GridJunctions[x, y].GridPoints[1];
                    gridPointB = GridJunctions[x, y].GridPoints[2];
                    gridPointC = GridJunctions[x, y].GridPoints[0];

                    if(GridPoint.GetCommonNeighbor(gridPointA,gridPointB,gridPointC,out neighbor))
                    {
                        if(neighbor!=null && neighbor.Hexagon!=null && neighbor.Hexagon.Activated
                            &&neighbor.Hexagon.ColorIndex==colorIndex)
                        {
                            numRemoved[neighbor.X]++;
                            lastRemoved[neighbor.X] = neighbor.Y > lastRemoved[neighbor.X] ? neighbor.Y : lastRemoved[neighbor.X];
                            neighbor.Hexagon.Deactivate();
                        }
                    }

                    ShiftGridPoints(ref numRemoved, ref lastRemoved);

                    //Skoru ekle.
                    int totalRemoved = 0;
                    for (int i = 0; i < numRemoved.Length; i++)
                        totalRemoved += numRemoved[i];
                    Menu.Instance.Score += totalRemoved * 5;

                    return true;
                }
            }
            
        }
        return false;
    }

    private void ShiftGridPoints(ref int[] numRemoved, ref int[] lastRemoved)
    {
        
        for (int x = 0; x < GridPoint.All.GetLength(0); x++)
        {
            
            if (numRemoved[x] < 1)
                continue;

            for(int y=1+lastRemoved[x]-numRemoved[x]; y<GridPoint.All.GetLength(1);y++)
            {
                if(y<GridPoint.All.GetLength(1)-numRemoved[x])
                {
                    GridPoint.All[x, y].Hexagon = GridPoint.All[x, y + numRemoved[x]].Hexagon;

                    GridPoint.All[x, y].Hexagon.Activated = false;

                    GridPoint.All[x, y].Hexagon.TimeActivated = Time.time;
                } else
                {
                    if(BombCounter<Menu.Instance.Score / BombAppearanceScore)
                    {
                        Bomb.CreateNew(GridPoint.All[x, y]);
                        BombCounter++;
                    } else
                    {
                        Hexagon.ActivatePooled(GridPoint.All[x, y]);
                    }
                }
            }
        }

    }

    public void InputCommands()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            if (Selection.SelectedGridJunction == null)
                Selection.Activate(Input.mousePosition);
            LastClickPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector3 dist = Input.mousePosition - LastClickPos;
            if(dist.magnitude>100f)
            {
                if(Selection.gameObject.activeInHierarchy)
                {
                    if(Mathf.Abs(dist.x)>Mathf.Abs(dist.y))
                    {
                        if (Input.mousePosition.x > LastClickPos.x)
                            Selection.RotateClockwise();
                        else
                            Selection.RotateCounterClockwise();
                    }
                    else
                    {
                        if (Input.mousePosition.y > LastClickPos.y)
                            Selection.RotateClockwise();
                        else
                            Selection.RotateCounterClockwise();
                    }
                }
            }
            else
            {
                Selection.Activate(Input.mousePosition);
                LastClickPos = Input.mousePosition;
            }
        }
        
        /*
        if(Input.touchCount>0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                if (Selection.SelectedGridJunction == null)
                    Selection.Activate(touch.position);
                LastClickPos = touch.position;

            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector3 dist = new Vector3(touch.position.x - LastClickPos.x,touch.position.y-LastClickPos.y,0);
                if (dist.magnitude > 100f)
                {
                    if (Selection.gameObject.activeInHierarchy)
                    {
                        if (Mathf.Abs(dist.x) > Mathf.Abs(dist.y))
                        {
                            if (touch.position.x > LastClickPos.x)
                                Selection.RotateClockwise();
                            else
                                Selection.RotateCounterClockwise();
                        }
                        else
                        {
                            if (touch.position.y > LastClickPos.y)
                                Selection.RotateClockwise();
                            else
                                Selection.RotateCounterClockwise();
                        }
                    }
                }
                else
                {
                    Selection.Activate(touch.position);
                    LastClickPos = touch.position;
                }
            }


        }
        */
        
    }

}
