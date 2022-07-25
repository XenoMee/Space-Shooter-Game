using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2.0f;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShot;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine, _leftEngine;

    private float _fireRate = 0.2f;
    private float _canFire = -1f;
    private int _lives = 3;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private PlayFabManager _playFabManager;

    [SerializeField]
    private AudioSource _laserAudio;
    [SerializeField]
    private AudioClip _laserShotClip;
    
    private int _score;
    private int _highscore;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL!");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if(_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL!");
        }

        _playFabManager = GameObject.Find("Canvas").GetComponent<PlayFabManager>();

        if (_playFabManager == null)
        {
            Debug.LogError("The PlayFab Manager is NULL!");
        }

        _laserAudio = GetComponent<AudioSource>();

        if(_laserAudio == null)
        {
            Debug.LogError("The AudioSource for the player's laser is NULL!");
        }
        else
        {
            _laserAudio.clip = _laserShotClip;
        }
    }

    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 distance = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(distance * _speed * Time.deltaTime);

        
        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {   
        _canFire = Time.time + _fireRate;

        var offset = new Vector3(0, 1.05f, 0);

        if(_isTripleShotActive == true)
        {
            Instantiate(_tripleShot, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + offset, Quaternion.identity);
        }

        _laserAudio.Play();

    }

    public void Damage()
    {
        if(_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(_isShieldActive);
            return;
        }

        _lives -= 1;

        if(_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if(_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);


        if(_lives == 0)
        {
            _spawnManager.onPlayerDeath();
            Destroy(this.gameObject);
            HighScore();
            _uiManager.GameOverSequence();
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(_isShieldActive);
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
          yield return new WaitForSeconds(5.0f);
          _isTripleShotActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
             yield return new WaitForSeconds(5.0f);
            _isSpeedBoostActive = false;
            _speed /= _speedMultiplier;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void HighScore()
    {
        _highscore = _score;
        _playFabManager.SendLeaderboard(_highscore);
    }
}

