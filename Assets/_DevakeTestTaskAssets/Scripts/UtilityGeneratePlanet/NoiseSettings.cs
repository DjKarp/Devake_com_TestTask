using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Класс настроек для разного вида слоёв генерации.
/// </summary>
[System.Serializable]
public class NoiseSettings
{

    /// <summary>
    /// 2 типа генерации возвышенностей.
    /// плавный и более острый
    /// </summary>
    public FilterType filterType;
    public enum FilterType
    {

        Simple,
        Rigid

    }

    /// <summary>
    /// В зависимости от выбранного типа генерации, отключает в инспекторе лишние пункты.
    /// 
    /// </summary>
    [ConditionalHide("filterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)]
    public RigidNoiseSettings rigidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings
    {

        public float strength = 1.0f;

        [Range(1, 8)]
        public int numLayers = 1;

        public float baseRoughness = 1.0f;
        public float roughness = 2.0f;

        public float persistence = 0.5f;

        public Vector3 centre;

        public float minValue;

    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {

        public float weightMultiplier = 0.8f;

    }

}
