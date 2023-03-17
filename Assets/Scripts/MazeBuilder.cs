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
        bool[,] grid =
        {
            {false, true, false},
            {true, true, true},
            {false, true, false},
        };

        MazeExtruder extruder = GetComponent<MazeExtruder>();
        strips = extruder.ExtrudeMaze(grid);

        foreach (WallStripInfo strip in strips)
        {
            MeshFilter filter = Instantiate(meshFilter);
            filter.mesh = strip.mesh;
        }
    }
}
