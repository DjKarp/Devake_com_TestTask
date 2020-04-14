using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Генерация меша
/// </summary>
public class TerrainFace
{

    private Mesh m_Mesh;
    private int resolution;

    private Vector3 localUp;
    private Vector3 axisX;
    private Vector3 axisY;

    private ShapeGeneration m_ShapeGeneration;

    private Vector3[] vertices;
    private int[] triangles;

    private int triIndex;


    public TerrainFace (ShapeGeneration shapeGeneration, Mesh mesh, int res, Vector3 locUp)
    {

        m_ShapeGeneration = shapeGeneration;

        m_Mesh = mesh;
        resolution = res;
        localUp = locUp;

        axisX = new Vector3(localUp.y, localUp.z, localUp.x);
        axisY = Vector3.Cross(localUp, axisX);

    }

    public void ConstructMesh()
    {

        vertices = new Vector3[resolution * resolution];
        triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3];
        triIndex = 0;

        for (int i = 0; i < resolution; i++)
        {

            for (int j = 0; j < resolution; j++)
            {

                int x = j + i * resolution;

                Vector2 percent = new Vector2(j, i) / (resolution - 1);

                Vector3 pointOnUnitCube = localUp + ((percent.x - 0.5f) * 2 * axisX) + ((percent.y - 0.5f) * 2 * axisY);
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                vertices[x] = m_ShapeGeneration.CalculatePointOnPlanet(pointOnUnitSphere);


                if (j != resolution - 1 && i != resolution - 1)
                {

                    triangles[triIndex] = x;
                    triangles[triIndex + 1] = x + resolution + 1;
                    triangles[triIndex + 2] = x + resolution;

                    triangles[triIndex + 3] = x;
                    triangles[triIndex + 4] = x + 1;
                    triangles[triIndex + 5] = x + resolution + 1;

                    triIndex += 6;

                }

            }

        }

        m_Mesh.Clear();
        m_Mesh.vertices = vertices;
        m_Mesh.triangles = triangles;
        m_Mesh.RecalculateNormals();

    }

}
