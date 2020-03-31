using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public GameObject BackgroundObj;
    public GameObject ForegroundObj;
    public GameObject Hexagon0;
    public GameObject Hexagon1;
    public GameObject Hexagon2;

    private SpriteRenderer Hexagon0SpriteRenderer;
    public Color Hexagon0Color
    #region Property 
    {
        get => Hexagon0SpriteRenderer.color;
        set => Hexagon0SpriteRenderer.color = value;
    }
    #endregion

    private SpriteRenderer Hexagon1SpriteRenderer;
    public Color Hexagon1Color
    #region Property 
    {
        get => Hexagon1SpriteRenderer.color;
        set => Hexagon1SpriteRenderer.color = value;
    }
    #endregion

    private SpriteRenderer Hexagon2SpriteRenderer;
    public Color Hexagon2Color
    #region Property 
    {
        get => Hexagon2SpriteRenderer.color;
        set => Hexagon2SpriteRenderer.color = value;
    }
    #endregion

    private SpriteRenderer Bomb0SpriteRenderer;
    private SpriteRenderer Bomb1SpriteRenderer;
    private SpriteRenderer Bomb2SpriteRenderer;

    [System.NonSerialized]
    public GridJunction SelectedGridJunction;

    private void Start()
    {
        GridMaker.Instance.Selection = this;

        Hexagon0SpriteRenderer = Hexagon0.GetComponent<SpriteRenderer>();
        Hexagon1SpriteRenderer = Hexagon1.GetComponent<SpriteRenderer>();
        Hexagon2SpriteRenderer = Hexagon2.GetComponent<SpriteRenderer>();

        Bomb0SpriteRenderer = Hexagon0.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        Bomb1SpriteRenderer = Hexagon1.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        Bomb2SpriteRenderer = Hexagon2.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        ForegroundObj.transform.parent = null;
        ForegroundObj.transform.localScale = Vector3.one * GridMaker.Instance.HexagonScale.x*12;
        transform.localScale = GridMaker.Instance.HexagonScale*12 ;
        
        //ForegroundObj.transform.localScale = Vector3.one*0.365f;
        //transform.localScale = Vector3.one*0.365f;

        Deactivate();
    }

    public void Reactivate()
    {
        Activate(Camera.allCameras[0].WorldToScreenPoint(transform.position));
    }

    public void Activate(Vector2 screenPoint)
    {
        Activate(new Vector3(screenPoint.x, screenPoint.y, 0));
    }

    public void Activate(Vector3 screenPoint)
    {
        if (GridMaker.Instance == null) return;

        Vector3 worldPoint = Camera.allCameras[0].ScreenToWorldPoint(screenPoint);

        //En yakın GridJunction'u bul.
        GridJunction closest = null;
        float closestDist = float.MaxValue;
        for(int x=0; x< GridMaker.Instance.GridJunctions.GetLength(0);x++)
        {
            for(int y = 0; y<GridMaker.Instance.GridJunctions.GetLength(1);y++)
            {
                float distance = Vector3.Distance(GridMaker.Instance.GridJunctions[x, y].WorldPosition, worldPoint);
                if (distance > closestDist)
                    continue;

                closest = GridMaker.Instance.GridJunctions[x, y];
                closestDist = distance;
            }
        }

        SelectedGridJunction = closest;

        gameObject.SetActive(true);
        ForegroundObj.SetActive(true);

        Bomb0SpriteRenderer.enabled = Bomb1SpriteRenderer.enabled = Bomb2SpriteRenderer.enabled = false;
        Hexagon0SpriteRenderer.enabled = Hexagon1SpriteRenderer.enabled = Hexagon2SpriteRenderer.enabled = true;

        //Pozisyon ayarlama
        ForegroundObj.transform.position = transform.position = SelectedGridJunction.WorldPosition;

        //Döndürme
        BackgroundObj.transform.localRotation = SelectedGridJunction.isOdd ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180);
        Hexagon0.transform.localPosition = SelectedGridJunction.isOdd ? new Vector3(-0.85f, 0) : new Vector3(0.85f, 0);
        Hexagon1.transform.localPosition = SelectedGridJunction.isOdd ? new Vector3(0.85f, -1f) : new Vector3(-0.85f, -1f);
        Hexagon2.transform.localPosition = SelectedGridJunction.isOdd ? new Vector3(0.85f, 1f) : new Vector3(-0.85f, 1f);

        //Renkler
        Hexagon0Color = SelectedGridJunction.GridPoints[0].Hexagon.Color;
        Hexagon1Color = SelectedGridJunction.GridPoints[1].Hexagon.Color;
        Hexagon2Color = SelectedGridJunction.GridPoints[2].Hexagon.Color;

        //Bomba kontrolü
        if(SelectedGridJunction.GridPoints[0].Hexagon is Bomb)
        {
            Hexagon0SpriteRenderer.enabled = false;
            Bomb0SpriteRenderer.enabled = true;
            Bomb0SpriteRenderer.color = Hexagon0SpriteRenderer.color;
        }

        if (SelectedGridJunction.GridPoints[1].Hexagon is Bomb)
        {
            Hexagon1SpriteRenderer.enabled = false;
            Bomb1SpriteRenderer.enabled = true;
            Bomb1SpriteRenderer.color = Hexagon1SpriteRenderer.color;
        }

        if (SelectedGridJunction.GridPoints[2].Hexagon is Bomb)
        {
            Hexagon2SpriteRenderer.enabled = false;
            Bomb2SpriteRenderer.enabled = true;
            Bomb2SpriteRenderer.color = Hexagon2SpriteRenderer.color;
        }

    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        ForegroundObj.SetActive(false);
    }

    private IEnumerator RotateEnum;
    public void RotateCounterClockwise() => StartCoroutine(RotateEnum = Rotate(1f));
    public void RotateClockwise() => StartCoroutine(RotateEnum = Rotate(-1f));

    private IEnumerator Rotate(float direction)
    {
        //Eğer bomba ise textmesh'i sakla. Hareketten sonra tekrar aktifleşecek.
        if(SelectedGridJunction.GridPoints[0].Hexagon is Bomb)
        {
            Bomb bomb = (Bomb)SelectedGridJunction.GridPoints[0].Hexagon;
            bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        if (SelectedGridJunction.GridPoints[1].Hexagon is Bomb)
        {
            Bomb bomb = (Bomb)SelectedGridJunction.GridPoints[1].Hexagon;
            bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        if (SelectedGridJunction.GridPoints[2].Hexagon is Bomb)
        {
            Bomb bomb = (Bomb)SelectedGridJunction.GridPoints[2].Hexagon;
            bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        //Gameready durumunu false yapıyoruz. Böylece hamle bitmeden oyuncular tekrar hamle yapamayacak.
        GridMaker.Instance.GameReady = false;

        //rotasyon zamanını üçe bölüyoruz, çünkü dönüş üç aşamadan oluşacak.
        float rotationTime = 1.0f / 3;

        for(int i = 0; i<3; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, direction * 120 * (i + 1));
            Quaternion startRotation = Quaternion.Euler(0, 0, direction * 120f * i);
            float startTime = Time.time;
            while(Time.time <startTime + rotationTime)
            {
                transform.rotation = Quaternion.Lerp(startRotation, rotation, (1f / rotationTime) * (Time.time - startTime));
                yield return null;
            }
            transform.rotation = rotation;

            if(direction >0)            
                SelectedGridJunction.SwitchHexagonsClockwise();
             else            
                SelectedGridJunction.SwitchHexagonsCounterClockwise();


            if (GridMaker.Instance.ExplosionOccurred = GridMaker.Instance.CheckForExplosion(SelectedGridJunction))
            {
                transform.rotation = Quaternion.identity;
                Deactivate();
                break;
            }

            yield return null;

        }

        if (GridMaker.Instance.ExplosionOccurred)
            Menu.Instance.NumOfMoves++;
        else
        {
            GridMaker.Instance.GameReady = true;

            //TextMeshRendererlar aktifleştirme.
            if(SelectedGridJunction.GridPoints[0].Hexagon is Bomb)
            {
                Bomb bomb = (Bomb)SelectedGridJunction.GridPoints[0].Hexagon;
                bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            if (SelectedGridJunction.GridPoints[1].Hexagon is Bomb)
            {
                Bomb bomb = (Bomb)SelectedGridJunction.GridPoints[1].Hexagon;
                bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            if (SelectedGridJunction.GridPoints[2].Hexagon is Bomb)
            {
                Bomb bomb = (Bomb)SelectedGridJunction.GridPoints[2].Hexagon;
                bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }

    }


}
