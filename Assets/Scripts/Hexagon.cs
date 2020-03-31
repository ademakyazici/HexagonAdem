using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{

    public static Queue<Hexagon> Unused = new Queue<Hexagon>();

    [HideInInspector]
    public int Index = -1;

    [SerializeField]
    private GridPoint gridPoint;
    public GridPoint GridPoint
    #region Property
    {
        get => gridPoint;
        set
        {
            LastGridPoint = gridPoint;
            gridPoint = value;
        }
    }
    #endregion
    [System.NonSerialized]
    private GridPoint LastGridPoint = null;
    
    public Vector3 GridPos => GridPoint.LocalPosition;
    public Vector3 GridPosWorld => GridPoint.GridMaker.transform.TransformPoint(GridPoint.LocalPosition);
    public Vector3 GridPosStart => GridPoint.LocalStartPosition;
    public Vector3 GridPosStartWorld => GridPoint.GridMaker.transform.TransformPoint(GridPoint.LocalStartPosition);

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer == null ? spriteRenderer = GetComponent<SpriteRenderer>() : spriteRenderer;
    public Color Color
    #region Property
    {
        get => SpriteRenderer.color;
        set
        {
            for(int i = 0; i<GridPoint.GridMaker.HexagonColors.Length;i++)
            {
                if(GridPoint.GridMaker.HexagonColors[i].Equals(value))
                {
                    colorIndex = i;                    
                    SpriteRenderer.color = GridPoint.GridMaker.HexagonColors[i];
                    break;
                }
            }
        }
    }
    #endregion   
    [SerializeField]
    private int colorIndex;
    public int ColorIndex => colorIndex;

    [System.NonSerialized]
    public float TimeActivated = -1;
    private float DeltaActivation => Time.time - TimeActivated;

    [System.NonSerialized]
    public bool Activated;

    public void ActivateInSeconds(float time)
    {
        Invoke("Activate", time);
    }
    
    public static bool ActivatePooled(GridPoint gridPoint)
    {
        if (Unused.Count < 1) return false;
        return Unused.Dequeue().Activate(gridPoint,true);
    }

    public bool Activate()
    {
        return Activate(null);
    }

    public bool Activate(GridPoint gridPoint, bool randomizeColor = false)
    {
        gameObject.SetActive(true);
        TimeActivated = Time.time;
        if (GridPoint == null)
        {
            gridPoint.Hexagon = this;
        }
        transform.localPosition = GridPosStart;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            TimeActivated = -1;
            transform.localPosition = GridPos;
        }
#endif
        if(randomizeColor)
            Color = GridMaker.Instance.HexagonColors[Random.Range(0, GridMaker.Instance.HexagonColors.Length)];

       
        return true;

    }

    public void Deactivate(bool preserveGridPoint = false)
    {
        gameObject.SetActive(false);
        TimeActivated = -1f;
        Activated = false;

        if(!preserveGridPoint)
        {
            GridPoint.Hexagon = null;
            GridPoint = null;
            LastGridPoint = null;
           
            if(this is Bomb)
            {
                Bomb bomb = (Bomb)this;
                if (Bomb.All.Contains(bomb))
                    Bomb.All.Remove(bomb);
                Destroy(gameObject);
            }
            else            
                Hexagon.Unused.Enqueue(this);
        }
    } 

    protected static GameObject CreateNewSprite(int i = -1)         
    {
        GameObject gameObject = new GameObject("Hexagon" + (i < 0 ? "" : i.ToString()));
        gameObject.SetActive(false);
        gameObject.transform.parent = GridMaker.Instance.transform;
        gameObject.transform.localScale = GridMaker.Instance.HexagonScale;

        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        return gameObject;
    }

    public static Hexagon CreateNew(int i)
    {
        Hexagon hexagon = CreateNewSprite(i).AddComponent<Hexagon>();
        hexagon.SpriteRenderer.sprite = GridMaker.Instance.HexagonSprite;
        GridMaker.Instance.Hexagons[i] = hexagon;
        hexagon.Index = i;

        return hexagon;

    }

    protected virtual void Update()
    {
        if (GridPoint == null) return;

        if(Activated)
        {
            transform.localPosition = GridPos;
            return;
        }
        if(LastGridPoint==null)
        {
            if (DeltaActivation < 1.0f)
                transform.localPosition = Vector3.Lerp(GridPosStart, GridPos, DeltaActivation);
            else
            {
                transform.localPosition = GridPos;
                Activated = true;
            }
        }
        else
        {
            float t = (1.0f / GridMaker.Instance.Size.y) * (LastGridPoint.Y - GridPoint.Y);
            if(DeltaActivation<t)
                transform.localPosition= Vector3.Lerp(LastGridPoint.LocalPosition, GridPos, (1.0f / t) * DeltaActivation);
            else
            {
                transform.localPosition = GridPos;
                Activated = true;
            }
        }
    }

    



}
