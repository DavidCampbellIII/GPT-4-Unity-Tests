using System.Collections.Generic;
using UnityEngine;

public class MazeExtruder : MonoBehaviour
{
    public float wallHeight = 3f;
    public float wallThickness = 1f;

    private Mesh CreateWallStripMesh(Vector3 start, Vector3 end)
    {
        Mesh mesh = new Mesh();

        Vector3 direction = (end - start).normalized;
        Vector3 normal = Vector3.Cross(Vector3.up, direction);
        Vector3 offset = normal * wallThickness * 0.5f;

        Vector3[] vertices = new Vector3[]
        {
            start - offset,
            start + offset,
            start + offset + new Vector3(0, wallHeight, 0),
            start - offset + new Vector3(0, wallHeight, 0),
            end - offset,
            end + offset,
            end + offset + new Vector3(0, wallHeight, 0),
            end - offset + new Vector3(0, wallHeight, 0)
        };

        int[] triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2,
            4, 5, 6,
            4, 6, 7,
            0, 4, 7,
            0, 7, 3,
            1, 0, 4,
            1, 4, 5,
            3, 6, 2,
            3, 7, 6,
            2, 5, 1,
            2, 6, 5
        };

        // Manually calculate normals
        Vector3[] normals = new Vector3[vertices.Length];

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i + 1]];
            Vector3 c = vertices[triangles[i + 2]];

            Vector3 faceNormal = Vector3.Cross(b - a, c - a).normalized;

            normals[triangles[i]] += faceNormal;
            normals[triangles[i + 1]] += faceNormal;
            normals[triangles[i + 2]] += faceNormal;
        }

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = normals[i].normalized;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.RecalculateUVDistributionMetrics();

        return mesh;
    }


    public List<WallStripInfo> ExtrudeMaze(bool[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        bool[,] visited = new bool[width, height];

        List<WallStripInfo> wallStrips = new List<WallStripInfo>();

       // //Create horizontal wall strips
       // for (int x = 0; x < width; x++)
       // {
       //     for (int y = 0; y < height; y++)
       //     {
       //         if (maze[x, y] && !visited[x, y])
       //         {
       //             Check horizontal wall strip
       //             int horizontalLength = 1;
       //             while (x + horizontalLength < width && maze[x + horizontalLength, y] && !visited[x + horizontalLength, y])
       //             {
       //                 horizontalLength++;
       //             }

       //             if (horizontalLength > 1)
       //             {
       //                 Vector2Int start = new Vector2Int(x, y);
       //                 Vector2Int end = new Vector2Int(x + horizontalLength, y);
       //                 float worldLength = horizontalLength * wallThickness;
       //                 Vector3 worldStart = new Vector3((start.x * wallThickness) - (wallThickness * 0.5f), 0, start.y * wallThickness);
       //                 Vector3 worldEnd = new Vector3((end.x * wallThickness) - (wallThickness * 0.5f), 0, end.y * wallThickness);
       //                 Mesh stripMesh = CreateWallStripMesh(worldStart, worldEnd);
       //                 WallStripInfo stripInfo = new WallStripInfo(start, end, horizontalLength, worldLength, stripMesh);
       //                 wallStrips.Add(stripInfo);
       //                 Mark visited cells(horizontal)
       //                 for (int i = 0; i < horizontalLength; i++)
       //                 {
       //                     visited[x + i, y] = true;
       //                 }
       //             }
       //         }
       //     }
       // }

       // //Reset visited array
       //visited = new bool[width, height];

       // //Create vertical wall strips
       // for (int x = 0; x < width; x++)
       // {
       //     for (int y = 0; y < height; y++)
       //     {
       //         if (maze[x, y] && !visited[x, y])
       //         {
       //             Check vertical wall strip
       //             int verticalLength = 1;
       //             while (y + verticalLength < height && maze[x, y + verticalLength] && !visited[x, y + verticalLength])
       //             {
       //                 Check if the current cell is already part of a horizontal wall strip
       //                 if (x > 0 && maze[x - 1, y + verticalLength])
       //                 {
       //                     break;
       //                 }
       //                 verticalLength++;
       //             }

       //             if (verticalLength > 1)
       //             {
       //                 Vector2Int start = new Vector2Int(x, y);
       //                 Vector2Int end = new Vector2Int(x, y + verticalLength);
       //                 float worldLength = verticalLength * wallThickness;
       //                 Vector3 worldStart = new Vector3(start.x * wallThickness, 0, (start.y * wallThickness) - (wallThickness * 0.5f));
       //                 Vector3 worldEnd = new Vector3(end.x * wallThickness, 0, (end.y * wallThickness) - (wallThickness * 0.5f));
       //                 Mesh stripMesh = CreateWallStripMesh(worldStart, worldEnd);
       //                 WallStripInfo stripInfo = new WallStripInfo(start, end, verticalLength, worldLength, stripMesh);
       //                 wallStrips.Add(stripInfo);
       //                 Mark visited cells(vertical)
       //                 for (int i = 0; i < verticalLength; i++)
       //                 {
       //                     visited[x, y + i] = true;
       //                 }
       //             }
       //         }
       //     }
       // }
       // return wallStrips;

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

                    if (horizontalLength >= 1)
                    {
                        Vector2Int start = new Vector2Int(x, y);
                        Vector2Int end = new Vector2Int(x + horizontalLength, y);
                        float worldLength = horizontalLength * wallThickness;
                        Vector3 worldStart = new Vector3((start.x * wallThickness) - (wallThickness * 0.5f), 0, start.y * wallThickness);
                        Vector3 worldEnd = new Vector3((end.x * wallThickness) - (wallThickness * 0.5f), 0, end.y * wallThickness);
                        Mesh stripMesh = CreateWallStripMesh(worldStart, worldEnd);
                        WallStripInfo stripInfo = new WallStripInfo(start, end, horizontalLength, worldLength, stripMesh);
                        wallStrips.Add(stripInfo);

                        // Mark visited cells
                        for (int i = 0; i < horizontalLength; i++)
                        {
                            visited[x + i, y] = true;
                        }
                    }

                    // Check vertical wall strip
                    int verticalLength = 0;
                    while (y + verticalLength < height && maze[x, y + verticalLength] && !visited[x, y + verticalLength])
                    {
                        verticalLength++;
                    }

                    if (verticalLength >= 1)
                    {
                        Vector2Int start = new Vector2Int(x, y);
                        Vector2Int end = new Vector2Int(x, y + verticalLength);
                        float worldLength = verticalLength * wallThickness;
                        Vector3 worldStart = new Vector3(start.x * wallThickness, 0, (start.y * wallThickness) - (wallThickness * 0.5f));
                        Vector3 worldEnd = new Vector3(end.x * wallThickness, 0, (end.y * wallThickness) - (wallThickness * 0.5f));
                        Mesh stripMesh = CreateWallStripMesh(worldStart, worldEnd);
                        WallStripInfo stripInfo = new WallStripInfo(start, end, verticalLength, worldLength, stripMesh);
                        wallStrips.Add(stripInfo);

                        // Mark visited cells
                        for (int i = 0; i < verticalLength; i++)
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
