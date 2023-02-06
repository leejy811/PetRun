using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Player player;
    public Text scoreText;
    public Text highScoreText;

    //panel
    public GameObject mainPanel;
    public GameObject inGamePanel;
    public GameObject howToPlayPanel;
    public GameObject gameOverPanel;

    public Text curScoreText;

    // 체력 관련
    //public Image[] Life;
    //public Sprite heart, noHeart;
    public Image healthImage;

    public GameObject[] jumpSlideButton;
    public bool isSlideButtonDown;

    //트로피 관련
    public Image trophyImage;
    public Sprite trophyIdleSprite;
    public Sprite[] trophyHighSprites;

    GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        /*
        for (int i = 0; i<player.maxHealth; i++)
        {
            Life[i].sprite = noHeart;
        }

        for (int i = 0; i<player.maxHealth; i++)
        {
            if (i<player.curHealth)
            {
                Life[i].sprite = heart;
            }
        }
        */

        if (player.curHealth > player.maxHealth * 0.5f)
            healthImage.color = Color.green;
        else if (player.curHealth > player.maxHealth * 0.2f)
            healthImage.color = Color.yellow;
        else
            healthImage.color = Color.red;

        healthImage.fillAmount = player.curHealth / player.maxHealth;

        scoreText.text = string.Format("{0:n0}", player.score);
    }

    public void Change()
    {
        if (!gameManager.isStart)
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

        gameManager.isReady = true;
        StartCoroutine(StartSet());
        gameManager.PlaySound("Go");
    }

    IEnumerator StartSet()
    {
        gameManager.countDown.transform.position = new Vector3(player.transform.position.x + 4, -2, 9);
        gameManager.countDown.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.startSoundSource.Play();
        yield return new WaitForSeconds(3.0f);
        gameManager.gunSoundSource.Play();
        yield return new WaitForSeconds(1.0f);
        gameManager.PlaySound("OnGameIntro");
        gameManager.isReady = false;
        gameManager.isStart = true;
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
        SceneManager.LoadScene(0);
    }
}
