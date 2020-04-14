using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// SO настроек генерируемого меша планеты
/// </summary>
[CreateAssetMenu(menuName = "GeneratePlanetSO/ShapeSettings")]
public class ShapeSettings : ScriptableObject
{

    public float planetRadius = 1;

    public NoiseSettings noiseSettings;

    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {

        public bool enabled = true;

        public bool useFirstLayerAsMask;

        public NoiseSettings noiseSettings;

    }

}
