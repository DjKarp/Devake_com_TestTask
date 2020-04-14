using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Наследованый от Pawn класс Врагов.
/// </summary>
public class EnemyPawn : Pawn
{

    //Скрипт сам выставляет уровень здоровья на полоске HP Bar.
    //И так как сам бар делается как спрайт на сцене (а не GUI элемент), то изменять размер полоски будем скейлом по оси. 
    //Для этого надо не забывать, что пивот полоски должен быть в нуле, т.е. в самой левой центральной части, если полоска уменьшается справа на лево.
    [Header("Sprite самой полоски HP Bar'а.")]
    [SerializeField]
    protected Transform hpBar;
    //Запоминаем начальный скейл полоски. Так как он может и не равняться еденице. Когда будем апромиксировать.
    protected float startHpBarValue;
    
    /// <summary>
    /// Несколько стейтов врага (сделал это здесь так как АИ одинаков для всех врагов пока)
    /// </summary>
    private EnemyState currentEnemyState;
    public enum EnemyState
    {

        walk,
        Attack,
        Die,
        Idle

    }

    private float distToPlayer;
    private float distPlayerAttack = 2.0f;

    private Vector3 moveDirection;

    private Vector3 attackPosition;
    private Quaternion attackRotation;

    private RaycastHit m_Hit;    

    private Rigidbody m_Rigidbody;

    private Vector3 tempPosition;



    protected override void Awake()
    {
        
        base.Awake();

        currentEnemyState = EnemyState.Idle;

        GameManager.Instance.allEnemyGO.Add(gameObject);
        GameManager.Instance.allEnemyPawns.Add(this);
        GameManager.Instance.allEnemyTR.Add(m_Transform);

        m_Rigidbody = gameObject.GetComponent<Rigidbody>();

        startHpBarValue = hpBar.localScale.x;
        
    }

    protected override void Update()
    {

        base.Update();

        CheckDistanceForPlayer();

        switch (currentEnemyState)
        {

            case EnemyState.walk:
                Move();
                break;

            case EnemyState.Attack:
                AttackPlayer();
                break;

            case EnemyState.Die:
                m_Rigidbody.angularVelocity = Vector3.zero;
                break;

        }

    }

    public override void TakeDamage(float damage)
    {

        base.TakeDamage(damage);

        SoundAndMusic.Instance.PlayEnemyDamage(gameObject);

    }

    protected override void CheckDie()
    {

        base.CheckDie();

        hpBar.localScale = new Vector3((startHpBarValue * HP) / maxHP, hpBar.localScale.y, hpBar.localScale.z);

    }

    private void Move()
    {
        
        moveDirection = GameManager.Instance.GetPlayerPosition() - m_Transform.position;
        m_Transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        m_Rigidbody.MovePosition(m_Transform.position + (moveDirection.normalized * Time.deltaTime * speed));

    }

    private void AttackPlayer()
    {

        m_Animator.SetTrigger("isHandAttack");

        m_Transform.position = attackPosition;
        m_Transform.rotation = attackRotation;

        //Нанесение сейчас урона на анимации аттаки Зомбака
        //GameManager.Instance.GetPlayerPawn().TakeDamage(weaponDamage);

    }


    public void SetPlayerDamage()
    {

        GameManager.Instance.GetPlayerPawn().TakeDamage(weaponDamage);

        SoundAndMusic.Instance.PlayEnemyAttack(gameObject);

    }

    private void CheckDistanceForPlayer()
    {

        distToPlayer = Vector3.Distance(m_Transform.position, GameManager.Instance.GetPlayerPosition());

        if (distToPlayer > distPlayerAttack && currentEnemyState != EnemyState.walk && !IsDie())
        {

            EnemyStateChange(EnemyState.walk);

        }
        else if (distToPlayer <= distPlayerAttack && currentEnemyState != EnemyState.Attack && !IsDie())
        {

            EnemyStateChange(EnemyState.Attack);
            attackPosition = m_Transform.position;
            attackRotation = m_Transform.rotation;
            m_Transform.rotation = Quaternion.Euler(new Vector3(0.0f, m_Transform.rotation.eulerAngles.y, 0.0f));

        }

    }

    protected override void Die()
    {

        base.Die();        

        EnemyStateChange(EnemyState.Die);
        
        GameManager.Instance.allEnemyGO.Remove(gameObject);
        GameManager.Instance.allEnemyPawns.Remove(this);
        GameManager.Instance.allEnemyTR.Remove(m_Transform);
        
        SoundAndMusic.Instance.PlayEnemyDie(gameObject);

        Destroy(gameObject, 3.0f);

    }

    private void EnemyStateChange(EnemyState enemyState)
    {

        currentEnemyState = enemyState;

        switch (enemyState)
        {

            case EnemyState.Idle:
                if (m_Animator.GetBool("isWalk")) m_Animator.SetBool("isWalk", false);
                break;

            case EnemyState.walk:
                if (!m_Animator.GetBool("isWalk")) m_Animator.SetBool("isWalk", true);
                break;

            case EnemyState.Attack:
                if (m_Animator.GetBool("isWalk")) m_Animator.SetBool("isWalk", false);
                m_Animator.SetTrigger("isHandAttack");
                break;

            case EnemyState.Die:
                m_Animator.SetTrigger("isDie");
                break;

        }

    }

}
