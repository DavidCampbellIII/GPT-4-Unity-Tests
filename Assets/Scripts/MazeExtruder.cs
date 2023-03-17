using System.Collections.Generic;
using UnityEngine;

public class MazeExtruder : MonoBehaviour
{
    public float wallHeight = 3f;
    public float wallThickness = 1f;

    private Mesh CreateWallStripMesh(Vector3 start, Vector3 end)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(start.x, 0, start.z),
            new Vector3(start.x, wallHeight, start.z),
            new Vector3(end.x, wallHeight, end.z),
            new Vector3(end.x, 0, end.z)
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public List<WallStripInfo> ExtrudeMaze(bool[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        bool[,] visited = new bool[width, height];

        List<WallStripInfo> wallStrips = new List<WallStripInfo>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] && !visited[x, y])
                {
                    // Check horizontal wall strip
                    int horizontalLength = 1;
                    while (x + horizontalLength < width && maze[x + horizontalLength, y] && !visited[x + horizontalLength, y])
                    {
                        horizontalLength++;
                    }

                    // Check vertical wall strip
                    int verticalLength = 1;
                    while (y + verticalLength < height && maze[x, y + verticalLength] && !visited[x, y + verticalLength])
                    {
                        verticalLength++;
                    }

                    int stripLength;
                    bool horizontal;

                    // Choose longer strip
                    if (horizontalLength > verticalLength)
                    {
                        horizontal = true;
                        stripLength = horizontalLength;
                    }
                    else
                    {
                        horizontal = false;
                        stripLength = verticalLength;
                    }

                    Vector2Int start = new Vector2Int(x, y);
                    Vector2Int end = horizontal ? new Vector2Int(x + stripLength - 1, y) : new Vector2Int(x, y + stripLength - 1);
                    float worldLength = stripLength * wallThickness;
                    Vector3 worldStart = new Vector3(start.x * wallThickness, 0, start.y * wallThickness);
                    Vector3 worldEnd = new Vector3(end.x * wallThickness, 0, end.y * wallThickness);
                    Mesh stripMesh = CreateWallStripMesh(worldStart, worldEnd);
                    WallStripInfo stripInfo = new WallStripInfo(start, end, stripLength, worldLength, stripMesh);
                    wallStrips.Add(stripInfo);

                    // Mark visited cells
                    for (int i = 0; i < stripLength; i++)
                    {
                        if (horizontal)
                        {
                            visited[x + i, y] = true;
                        }
                        else
                        {
                            visited[x, y + i] = true;
                        }
                    }
                }
            }
        }
        return wallStrips;
    }
}

public class WallStripInfo
{
    public Vector2Int start;
    public Vector2Int end;
    public Vector2Int center;
    public int length;
    public float worldLength;
    public Mesh mesh;

    public WallStripInfo(Vector2Int start, Vector2Int end, int length, float worldLength, Mesh mesh)
    {
        this.start = start;
        this.end = end;
        this.center = (start + end) / 2;
        this.length = length;
        this.worldLength = worldLength;
        this.mesh = mesh;
    }
}
