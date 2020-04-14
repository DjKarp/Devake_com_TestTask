using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Генерация планеты сделана по туториалу от https://www.youtube.com/user/Cercopithecan/
/// 
/// С моими доработками и пояснениями.
/// </summary>

public class Planet : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;

    public bool autoUpdate = true;

    public ShapeSettings m_ShapeSettings;
    public ColourSettings m_ColourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldOut;
    [HideInInspector]
    public bool colorSettingsFoldOut;

    private ShapeGeneration m_ShapeGeneration;

    private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    private Material planetMaterial;



    private void OnValidate()
    {

        planetMaterial = Resources.Load("Materials/StandartMaterial") as Material;

        m_ShapeSettings = Resources.Load("Settings/Shape") as ShapeSettings;
        m_ColourSettings = Resources.Load("Settings/Colour") as ColourSettings;
        
    }


    void Initialize()
    {

        m_ShapeGeneration = new ShapeGeneration(m_ShapeSettings);

        if (meshFilters == null || meshFilters.Length == 0) meshFilters = new MeshFilter[6];

        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {

            if (meshFilters[i] == null)
            {

                GameObject meshObj = new GameObject("PlanetGenerateMesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = planetMaterial;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();

            }

            terrainFaces[i] = new TerrainFace(m_ShapeGeneration, meshFilters[i].sharedMesh, resolution, directions[i]);

        }

    }

    public void GeneratePlanet()
    {

        Initialize();
        GenerateMesh();
        GenerateColours();

    }

    public void OnShapeSettingsUpdated()
    {

        if (autoUpdate)
        {

            Initialize();
            GenerateMesh();

        }

    }

    public void OnColourSettingsUpdated()
    {

        if (autoUpdate)
        {

            Initialize();
            GenerateColours();

        }

    }

    void GenerateMesh()
    {

        foreach(TerrainFace face in terrainFaces)
        {

            face.ConstructMesh();

        }

    }

    void GenerateColours()
    {

        foreach (MeshFilter mf in meshFilters)
        {

            mf.GetComponent<MeshRenderer>().sharedMaterial.color = m_ColourSettings.planetColour;

        }

    }

}
