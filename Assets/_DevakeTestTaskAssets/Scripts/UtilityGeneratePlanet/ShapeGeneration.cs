using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Вычисление высот точек меша 
/// </summary>
public class ShapeGeneration
{

    private ShapeSettings settings;
    private INoiseFilter[] noiseFilters;

    private float firstLayerValue;
    private float elevation;
    private float mask;


    public ShapeGeneration(ShapeSettings shapeSettings)
    {

        settings = shapeSettings;

        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];

        for (int i = 0; i < noiseFilters.Length; i++)
        {

            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);

        }

    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {

        firstLayerValue = 0;
        elevation = 0;

        if (noiseFilters.Length > 0)
        {

            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);

            if (settings.noiseLayers[0].enabled)
            {

                elevation = firstLayerValue;

            }

        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {

            if (settings.noiseLayers[i].enabled)
            {

                mask = settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;

            }

        }

        return pointOnUnitSphere * settings.planetRadius * (1 + elevation);

    }

}
