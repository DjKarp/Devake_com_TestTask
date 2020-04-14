using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Хранение данных о свойствах и характеристиках игрока, врагов и т.п..
/// Создание вынесено в своё меню.
/// 
/// Сделал его, чтобы игра была расширяемой.
/// </summary>
[CreateAssetMenu(menuName = "DesingSO/PawnSO")]
public class PawnSO : ScriptableObject
{

    [Header("Имя.")]
    [SerializeField]
    public string Name;

    [Header("Максимальное количество здоровья.")]
    [SerializeField]
    public float MaxHP;

    [Header("Наносимый урон.")]
    [SerializeField]
    public float Damage;

    [Header("Скорость передвижения.")]
    [SerializeField]
    public float Speed;

}
