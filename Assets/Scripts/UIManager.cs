using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private Text _score;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Text _gameOver;
    [SerializeField]
    private Text _restartGame;
    [SerializeField]
    private Button _playRewardedAd;

    private GameManager _gameManager;
    private AdsManager _adsManager;

    void Start()
    {
        _score.text = "Score: ";
        _gameOver.gameObject.SetActive(false);
        _restartGame.gameObject.SetActive(false);
        _playRewardedAd.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL!");
        }

        _adsManager = GameObject.Find("Canvas").GetComponent<AdsManager>();

        if(_adsManager == null)
        {
            Debug.LogError("The Ad Manager is NULL!");
        }
    }

    public void UpdateScore(int playerscore)
    {
        _score.text = "Score: " + playerscore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _livesSprites[currentLives];
    }

    public void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOver.gameObject.SetActive(true);
        _restartGame.gameObject.SetActive(true);
#if UNITY_IOS || UNITY_ANDROID
        _playRewardedAd.gameObject.SetActive(true);
#else
        _playRewardedAd.gameObject.SetActive(false);
#endif
        StartCoroutine(FlashingTextRoutine());
        _adsManager.PlayAd();

    }

    IEnumerator FlashingTextRoutine()
    {
        while(_gameOver != null)
        {
            _gameOver.text = " GAME OVER ";
            yield return new WaitForSeconds(0.75f);
            _gameOver.text = "";
            yield return new WaitForSeconds(0.75f);
        }
    }
}
