using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //컴포넌트 관련
    [SerializeField] Player player;
    GameManager gameManager;
    AdManager adManager;

    //Text
    [SerializeField] Text scoreText;
    [SerializeField] Text highScoreText;
    [SerializeField] Text curScoreText;

    //panel
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject inGamePanel;
    [SerializeField] GameObject howToPlayPanel;
    [SerializeField] GameObject gameOverPanel;

    //체력 관련
    [SerializeField] Gradient gradient;
    [SerializeField] Image healthImage;

    //트로피 관련
    [SerializeField] Image trophyImage;
    [SerializeField] Sprite trophyIdleSprite;
    [SerializeField] Sprite[] trophyHighSprites;

    //버튼 관련
    [SerializeField] GameObject[] jumpSlideButton;
    public bool isSlideButtonDown;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        adManager = GetComponent<AdManager>();
    }

    void Update()
    {
        float helathRatio = player.curHealth / player.maxHealth;
        healthImage.color = gradient.Evaluate(helathRatio);

        healthImage.fillAmount = helathRatio;

        scoreText.text = string.Format("{0:n0}", player.score);
    }

    public void Change()
    {
        if (!gameManager.isStart || player.isJump || player.isSlide)
            return;

        ChangeButton();

        player.Change(true);
    }

    public void ChangeButton()
    {
        int curIndex = jumpSlideButton[0].activeSelf == true ? 0 : 1;
        int nextIndex = curIndex == 0 ? 1 : 0;

        jumpSlideButton[curIndex].SetActive(false);
        jumpSlideButton[nextIndex].SetActive(true);
    }

    public void SlideButton(bool isDown)
    {
        if (!gameManager.isStart)
            return;

        isSlideButtonDown = isDown;

        Image button = jumpSlideButton[1].GetComponent<Image>();
        if (isSlideButtonDown)
            button.color = Color.gray;
        else
            button.color = Color.white;

    }

    public void GameStart()
    {
        mainPanel.SetActive(false);
        howToPlayPanel.SetActive(true);

        gameManager.PlaySound("Start");
    }

    public void InGame()
    {
        howToPlayPanel.SetActive(false);
        inGamePanel.SetActive(true);

        adManager.LoadInterstitialAd();
        StartCoroutine(gameManager.StartSet());
    }


    public void GameOver()
    {
        inGamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        int curScore = (int)player.score;
        int highScore = (int)PlayerPrefs.GetFloat("HighScore");

        if (player.isHighScore)
        {
            curScoreText.text = string.Format("{0:n0}", highScore);
            StartCoroutine(TrophyAnimation());
            gameManager.PlaySound("HighScore");
        }
        else
        {
            curScoreText.text = string.Format("{0:n0}", curScore);
            trophyImage.sprite = trophyIdleSprite;
            gameManager.PlaySound("GameOver");
        }

        highScoreText.text = string.Format("{0:n0}", highScore);
    }

    IEnumerator TrophyAnimation()
    {
        foreach(Sprite trophySprite in trophyHighSprites)
        {
            trophyImage.sprite = trophySprite;
            yield return new WaitForSeconds(0.083f);
        }

        StartCoroutine(TrophyAnimation());
    }

    public void Restart()
    {
        adManager.ShowAd();
    }
}
