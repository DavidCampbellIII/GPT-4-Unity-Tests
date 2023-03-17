using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;

    private List<WallStripInfo> strips;

    private void Start()
    {
        bool[,] grid1 =
        {
            {false, false, false},
            {true, true, true},
            {false, false, false},
            {true, true, true},
            {false, false, false},
            {true, true, true},
            {false, false, false},
        };

        bool[,] grid2 =
        {
            {true, true, true},
            {false, true, false},
            {false, true, false},
            {true, true, true},
            {false, false, false},
            {true, true, true},
        };

        bool[,] grid3 =
        {
            {true, true, true},
            {true, false, true},
            {true, false, true},
            {true, false, true},
            {true, false, true},
            {true, true, true},
        };

        //CreateMesh(grid1);
        //CreateMesh(grid2);
        CreateMesh(grid3);
    }

    private void CreateMesh(bool[,] grid)
    {
        MazeExtruder extruder = GetComponent<MazeExtruder>();
        strips = extruder.ExtrudeMaze(grid);

        foreach (WallStripInfo strip in strips)
        {
            MeshFilter filter = Instantiate(meshFilter);
            filter.mesh = strip.mesh;
        }
    }
}
