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

    //카운트다운 관련
    public GameObject num3;
    public GameObject num2;
    public GameObject num1;
    public GameObject go;

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

    void ChangeButton()
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
    }

    public void InGame()
    {
        howToPlayPanel.SetActive(false);
        inGamePanel.SetActive(true);

        StartCoroutine(StartSet());
    }

    IEnumerator StartSet()
    {
        num3.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        num3.SetActive(false);
        num2.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        num2.SetActive(false);
        num1.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        num1.SetActive(false);
        go.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        go.SetActive(false);
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
        }
        else
        {
            curScoreText.text = string.Format("{0:n0}", curScore);
            trophyImage.sprite = trophyIdleSprite;
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
