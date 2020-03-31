#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Hexagon))]
public class HexagonEditor : Editor
{
    Hexagon hexagon;
    Texture2D[] ColoredButtons;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
            EditorGUILayout.HelpBox("Oyun oynanırken düzenleme yapılamaz.", MessageType.Info);
        else
        {
            EditorGUILayout.HelpBox("Renk değerlerini Sprite renderer'dan değiştirmeyiniz.\nAşağıdaki renkli butonları kullanınız.", MessageType.Warning);

            for (int i = 0; i < hexagon.GridPoint.GridMaker.HexagonColors.Length; i++)
            {
                GUIStyle gUIStyle = new GUIStyle();
                gUIStyle.normal.background = ColoredButtons[i];
                if (GUILayout.Button("", gUIStyle)) 
                {
                    hexagon.Color = hexagon.GridPoint.GridMaker.HexagonColors[i];
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }
            }
            {

            }
        }
    }

    private void OnEnable()
    {
        hexagon = (Hexagon)target;
        ColoredButtons = new Texture2D[hexagon.GridPoint.GridMaker.HexagonColors.Length];
        for (int i = 0; i < ColoredButtons.Length; i++)
        {
            ColoredButtons[i] = new Texture2D(4, 4, TextureFormat.ARGB32, false);

            Color[] colors = new Color[16];
            for (int y = 0; y < colors.Length; y++)
                colors[y] = hexagon.GridPoint.GridMaker.HexagonColors[i];

            ColoredButtons[i].SetPixels(colors);
            ColoredButtons[i].Apply(false);
            {

            }
        }
    }
}
#endif