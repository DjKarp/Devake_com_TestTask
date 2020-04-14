using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Наследованый от Pawn класс игрока.
/// </summary>
public class HeroPawn : Pawn
{

    [Header("Точка откуда будут выстреливаться пули")]
    [SerializeField]
    private Transform shootPointTR;
    private GameObject shootPointGO;
    
    /// <summary>
    /// Будем делать Пул снарядов, чтобы не создавать их, а брать из заранее созданного и подготовленного списка.
    /// </summary>
    private GameObject bulletPoolParent;
    private List<GameObject> bulletPoolGO = new List<GameObject>();
    private List<Transform> bulletPoolTR = new List<Transform>();
    private List<BulletAI> bulletPool_AI = new List<BulletAI>();
    private int maxBulletPool = 50;
    
    private FP_Controller m_FP_Controller;

    private BulletAI currentBulletAI;

    /// <summary>
    /// Решил добавить минимальную перезарядку, чтобы не сильно спамить снарядами.
    /// </summary>
    private float reloadTimer;
    private float reloadTime = 0.3f;

    private GameObject tempGO;

    protected override void Awake()
    {
        
        base.Awake();

        bulletPoolParent = new GameObject("BulletPoolParent");

        tempGO = Resources.Load("Pawn/Bullet") as GameObject;

        for (int i = 0; i < maxBulletPool; i++)
        {

            bulletPoolGO.Add(Instantiate(tempGO, bulletPoolParent.transform));
            bulletPoolTR.Add(bulletPoolGO[bulletPoolGO.Count - 1].GetComponent<Transform>());
            bulletPool_AI.Add(bulletPoolGO[bulletPoolGO.Count - 1].GetComponent<BulletAI>());
            bulletPoolGO[bulletPoolGO.Count - 1].SetActive(false);

        }

        if (shootPointTR == null) shootPointTR = gameObject.transform;
        shootPointGO = shootPointTR.gameObject;

        m_FP_Controller = gameObject.GetComponent<FP_Controller>();   
        
    }

    private void Start()
    {

        GameManager.Instance.GetGUI_Manager().SetHPBarValue();

        ShowBullet();

    }

    protected override void Update()
    {
        
        base.Update();

        CheckShootAndSetBulletPosition();

        if (reloadTimer < reloadTime) reloadTimer += Time.deltaTime;

    }

    public override void TakeDamage(float damage)
    {

        base.TakeDamage(damage);

        PlaySound("damage");

        GameManager.Instance.GetGUI_Manager().SetHPBarValue();

    }
    /// <summary>
    /// Задаём положение ещё не выстреленой пули в точке выстрела. И ждём самого выстрела от игрока.
    /// </summary>
    private void CheckShootAndSetBulletPosition()
    {

        currentBulletAI.SetBulletPosition(shootPointTR.position);

        if (m_FP_Controller.GetPlayerInput().Shoot() | Input.GetKeyDown(KeyCode.F) | Input.GetKeyDown(KeyCode.Mouse0)) Shoot();

    }

    /// <summary>
    /// Нахождение выключеной не занятой пули и помещение её в слот для выстрела.
    /// </summary>
    private void ShowBullet()
    {

        for (int i = 0; i < bulletPoolGO.Count; i++)
        {

            if (!bulletPoolGO[i].activeSelf)
            {

                bulletPoolGO[i].SetActive(true);
                bulletPoolTR[i].position = shootPointTR.position;
                currentBulletAI = bulletPool_AI[i];
                
                break;

            }

        }

    }

    public void Shoot()
    {

        if (reloadTimer >= reloadTime)
        {

            reloadTimer = 0.0f;

            PlaySound("shoot");
            currentBulletAI.SetMoveDirectionAndDamage(GameManager.Instance.GetCameraMain().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 20.0f)), weaponDamage);
            currentBulletAI = null;

            ShowBullet();

        }

    }

    private void OnCollisionEnter(Collision collision)
    {

       

    }
    
    protected override void Die()
    {

        base.Die();

        GameManager.Instance.ChangeGameMode(GameManager.GameMode.Loose);

    }

    private void PlaySound(string st)
    {

        if (SoundAndMusic.Instance != null)
        {

            switch (st)
            {

                case "shoot":
                    SoundAndMusic.Instance.PlayPlayerShoot(shootPointGO);
                    break;

                case "damage":
                    SoundAndMusic.Instance.PlayPlayerDamage(gameObject);
                    break;

            }

        }

    }

}
