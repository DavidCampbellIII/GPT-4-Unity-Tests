using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    [SerializeField]
    private Gradient wallStripGradient;
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

            {true, true, true},
            {true, false, true},
            {true, false, true},
            {true, false, true},
            {true, false, true},
            {true, true, true},

            {true, true, true},
            {true, false, true},
            {true, false, true},
            {true, false, true},
            {true, false, true},
            {true, true, true},
        };

        bool[,] grid4 = new bool[,]
        {
            {false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
            {true, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, true},
            {true, false, true, true, true, true, true, true, false, true, true, true, false, true, false, true, true, true, true, true, true, true, false, true, true},
            {true, false, false, false, false, false, false, true, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, true},
            {true, true, true, true, true, true, false, true, false, true, false, true, true, true, true, true, true, true, true, true, true, true, false, true, true},
            {true, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
            {true, false, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true},
            {true, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
            {true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true},
            {true, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, true},
            {true, false, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, false, true, true, true, true, false, true, true},
            {true, false, true, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, true},
            {true, false, true, true, true, true, true, true, true, true, true, false, true, true, true, false, false, true, true, true, true, true, true, true, true},
            {true, false, true, false, false, false, false, false, false, false, false, true, false, true, false, false, false, false, false, false, false, false, false, false, true},
            {true, true, true, true, true, true, false, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, false, true, true},
            {true, false, false, false, false, false, false, false, false, false, false, true, false, true, false, true, false, false, false, false, false, false, false, false, true},
            {true, false, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true},
            {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
            {true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true},
            {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
            {true, false, true, true, true, true, true, true, false, false, true, true, true, true, true, true, true, true, false, true, true, true, false, true, true},
            {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, true},
            {true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true}
        };

        //CreateMesh(grid1);
        //CreateMesh(grid2);
        //CreateMesh(grid3);
        //CreateMesh(grid4);
        CreateMesh(LoadGridFromFile());
    }

    private bool[,] LoadGridFromFile()
    {
        string path = Application.dataPath + "/DebugGrid.txt";
        string[] lines = File.ReadAllLines(path);
        bool[,] grid = new bool[lines.Length, lines[0].Length];
        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                grid[i, j] = lines[i][j] == '1';
            }
        }
        return grid;
    }

    private void CreateMesh(bool[,] grid)
    {
        MazeExtruder extruder = GetComponent<MazeExtruder>();
        strips = extruder.ExtrudeMaze(grid);
        Debug.Log($"Num wall strips: {strips.Count}");

        foreach (WallStripInfo strip in strips)
        {
            MeshFilter filter = Instantiate(meshFilter);
            filter.mesh = strip.mesh;

            Color color = wallStripGradient.Evaluate(Random.Range(0f, 1f));
            filter.GetComponent<MeshRenderer>().material.color = color;
        }
    }
}
