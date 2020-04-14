using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Класс мягкого \ плавного подбора высоты поверхности
/// </summary>
public class SimpleNoiseFilter : INoiseFilter
{

    private NoiseSettings.SimpleNoiseSettings settings;

    private Noise noise = new Noise();

    private float noiseValue;
    private float frequency;
    private float amplitude;

    private float tempFloat;

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {

        this.settings = settings;

    }

    public float Evaluate(Vector3 point)
    {

        noiseValue = 0;
        frequency = settings.baseRoughness;
        amplitude = 1.0f;

        for (int i = 0; i < settings.numLayers; i++)
        {

            tempFloat = noise.Evaluate(point * frequency + settings.centre);

            noiseValue += (tempFloat + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;

        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);

        return noiseValue * settings.strength;

    }
    
}
