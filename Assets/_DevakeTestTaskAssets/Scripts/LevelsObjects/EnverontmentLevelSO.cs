using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Предметы окружения для создания уровня.
/// 
/// При создании уровней в разном стиле, нужно будет лишь подключать разные дизайны SO
/// </summary>
[CreateAssetMenu(menuName = "DesingSO/EverontmentLevelSO")]
public class EnverontmentLevelSO : ScriptableObject
{

    [Header("Материал для созданного ландшафта.")]
    [SerializeField]
    public Material TerrainMaterial;

    [Header("Список префабов объектов окружения.")]
    [SerializeField]
    public GameObject[] EnverontmentPrefabs;

}
