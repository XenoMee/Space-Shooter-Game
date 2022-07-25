using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;
    private Animator _enemyAnimator;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1.0f;
    private bool _isEnemyDead = false;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

        if(_player == null)
        {
            Debug.LogError("The Player is NULL!");
        }

        _enemyAnimator = gameObject.GetComponent<Animator>();

        if(_enemyAnimator == null)
        {
            Debug.LogError("The Animator is NULL!");
        }
    }

    void Update()
    {
        CalculateMovement();             
        FireLaser();       
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.30f)
        {
            float randomX = Random.Range(-9.50f, 9.50f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {

            if(_player != null)
            {
                _player.Damage();
            }
            
            _enemyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 2.8f);
            _isEnemyDead = true;
        }
        
        if(other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if(_player != null)
            {
                _player.AddScore(Random.Range(5, 10));
            }
           
            _enemyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
            _isEnemyDead = true;
        }
    }

    void FireLaser()
    {
        if (Time.time > _canFire && _isEnemyDead == false)
        {
            _fireRate = 2.8f;
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }
}
