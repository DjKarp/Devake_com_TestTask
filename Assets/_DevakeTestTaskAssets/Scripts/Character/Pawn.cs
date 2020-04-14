using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Основа для всех персонажей. Общие переменные и методы.
/// </summary>
public class Pawn : MonoBehaviour
{

    //Трансформ самого Pawn
    protected Transform m_Transform;

    //Текущий и максимальный уровни здоровья
    [SerializeField]
    protected float HP;
    protected float maxHP;

    protected string pawnName;
    protected float weaponDamage;
    protected float speed;

    // ScriptableObject содержащий характеристики персонажа и врагов - имя, урон, скорость и т.п.
    public PawnSO m_PawnSO;

    protected Animator m_Animator;
    public enum PawnAnimatorState
    {

        Idle,
        Walk,
        Run

    }


    protected virtual void Awake()
    {
        // Если в инспекторе не добавлен нужный ScriptableObject, то грузим стандартный
        if (m_PawnSO == null) m_PawnSO = Resources.Load("SO/DefaultDesingSO") as PawnSO;

        m_Animator = gameObject.GetComponent<Animator>();
        if (m_Animator == null) m_Animator = gameObject.GetComponentInChildren<Animator>();

        //Задаём переменные
        maxHP = m_PawnSO.MaxHP;
        HP = maxHP;

        pawnName = m_PawnSO.Name;
        weaponDamage = m_PawnSO.Damage;
        speed = m_PawnSO.Speed;
               
        m_Transform = gameObject.transform;               

        //Подписываемся на эвент смены режима игры
        GameManager.Instance.changeGameModeEvent += OnGameModeChanged;

    }

    protected virtual void Update()
    {


    }

    public virtual void TakeDamage(float damage)
    {

        HP = Mathf.Clamp(HP - damage, 0.0f, maxHP);

        CheckDie();

    }

    protected virtual void CheckDie()
    {

        if (IsDie())
        {

            Die();

        }

    }

    public virtual bool IsDie()
    {

        if (HP <= 0.0f) return true;
        else return false;

    }

    protected virtual void Die()
    {


    }

    protected virtual void OnGameModeChanged()
    {

        switch (GameManager.Instance.CurrentGameMode)
        {

            case GameManager.GameMode.Game:

                break;

        }

    }

    public void SetAnimationState(PawnAnimatorState pawnAnimatorState)
    {

        switch (pawnAnimatorState)
        {

            case PawnAnimatorState.Idle:
                m_Animator.SetBool("isWalk", false);
                m_Animator.SetBool("isRun", false);
                break;

            case PawnAnimatorState.Walk:
                m_Animator.SetBool("isWalk", true);
                break;

            case PawnAnimatorState.Run:
                m_Animator.SetBool("isRun", true);
                break;

        }

    }

    public float GetCurrentHP() { return HP; }
    public float GetMaxtHP() { return maxHP; }

}
