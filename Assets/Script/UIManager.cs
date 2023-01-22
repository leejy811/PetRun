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

    //Jump Slide 버튼
    public Sprite jump, slide;
    public Image jumpSlideButton;

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
        /* highscore
        highScoreText.text = string.Format("{0:n0}", player.highscore);
        */
        scoreText.text = string.Format("{0:n0}", player.score);

        if(player.curHealth == 0)
        {
            GameOver();
        }

        Change();
    }

    public void Change()
    {
        if(player.animalType == Player.AnimalType.Dog)
        {
            jumpSlideButton.sprite = jump;
        }
        else
        {
            jumpSlideButton.sprite = slide;
        }
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
    }

    public void GameOver()
    {
        inGamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        curScoreText.text = scoreText.text;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
