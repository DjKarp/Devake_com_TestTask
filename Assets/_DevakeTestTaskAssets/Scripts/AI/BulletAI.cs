using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Скрипт снаряда и его взаимодействия с окружающим миром.
/// </summary>
public class BulletAI : MonoBehaviour
{

    private Transform m_Transform;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 moveDirection;

    private float bulletSpeed = 3.0f;

    private float bulletDamage = 5.0f;

    private bool isShoot = false;

    private ParticleSystem m_ParticleSystem;


    private void Awake()
    {

        m_Transform = gameObject.transform;

        m_ParticleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        m_ParticleSystem.Stop();
        
    }

    private void LateUpdate()
    {
        
        if (isShoot)
        {

            m_Transform.Translate(moveDirection * Time.deltaTime * bulletSpeed);

            CheckBulletSoFar();

        }

    }

    public void SetBulletPosition(Vector3 startMovePos)
    {

        startPosition = startMovePos;
        m_Transform.position = startPosition;
        m_ParticleSystem.Stop();

    }

    public void SetMoveDirectionAndDamage(Vector3 direction,  float damag)
    {
                
        targetPosition = direction;
        moveDirection = (targetPosition - m_Transform.position);

        bulletDamage = damag;

        m_ParticleSystem.Play();

        isShoot = true;

    }

    private void CheckBulletSoFar()
    {

        if (Vector3.Distance(m_Transform.position, startPosition) > 100.0f) SetOffBullet();

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {

            GameManager.Instance.PlayerBulletHitInEnemy(other.gameObject, bulletDamage);
            SetOffBullet();

        }
        else if (other.gameObject.name == "Terrain") SetOffBullet();
        else if (other.gameObject.tag == "Enverontment") SetOffBullet();

    }
    private void SetOffBullet()
    {

        isShoot = false;
        m_ParticleSystem.Stop();
        gameObject.SetActive(false);

    }

}
