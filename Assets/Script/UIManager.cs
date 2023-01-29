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
    public Image[] Life;
    public Sprite heart, noHeart;

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
        yield return new WaitForSeconds(3f);
        gameManager.isStart = true;
    }

    public void GameOver()
    {
        inGamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        curScoreText.text = scoreText.text;
        highScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetFloat("HighScore"));

        if (player.isHighScore)
            StartCoroutine(TrophyAnimation());
        else
            trophyImage.sprite = trophyIdleSprite;
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
