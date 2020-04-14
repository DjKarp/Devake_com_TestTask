using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Вычисление острых \ жёстких пик ландшафта
/// </summary>
public class RigidNoiseFilter : INoiseFilter
{

    private NoiseSettings.RigidNoiseSettings settings;
    private Noise noise = new Noise();

    private float noiseValue;
    private float frequency;
    private float amplitude;
    private float weight;

    private float tempFloat;


    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
    {

        this.settings = settings;

    }

    public float Evaluate(Vector3 point)
    {

        noiseValue = 0;
        frequency = settings.baseRoughness;
        amplitude = 1.0f;
        weight = 1.0f;

        for (int i = 0; i < settings.numLayers; i++)
        {

            tempFloat = 1 - Mathf.Abs( noise.Evaluate(point * frequency + settings.centre) );

            tempFloat *= tempFloat;
            tempFloat *= weight;
            weight = Mathf.Clamp01(tempFloat * settings.weightMultiplier);

            noiseValue += tempFloat * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;

        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);

        return noiseValue * settings.strength;

    }

}
