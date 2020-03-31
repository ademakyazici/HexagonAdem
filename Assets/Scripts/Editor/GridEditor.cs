using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


[CustomEditor(typeof(GridMaker))]
public class GridEditor : Editor
{
    GridMaker grid;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if (check.changed && false)
                grid.GenerateGrid();
        }
            

        if(!Application.isPlaying && GUILayout.Button("Generate Grid"))
        {
            grid.GenerateGrid();           
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }

        if (!Application.isPlaying && GUILayout.Button("Remove Grid"))
        {
            grid.RemoveGrid();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }


    }

    private void OnEnable()
    {
        grid = (GridMaker)target;
    }
}
