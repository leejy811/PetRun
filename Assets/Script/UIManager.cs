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
    public GameObject mainpanel;
    public GameObject ingamepanel;
    public GameObject howtoplaypanel;
    public GameObject gameoverpanel;

    public Text curscoreText;

    // 체력 관련
    public Image[] Life;
    public int maxhealth;
    public int curhealth;
    public Sprite Heart, NoHeart;

    //Jump Slide 버튼
    public Sprite jump, slide;
    public Image jumpslidebutton;

    private void Start()
    {
        maxhealth = player.maxHealth;
    }
    void Update()
    {
        curhealth = player.curHealth;

        for (int i = 0; i<maxhealth; i++)
        {
            Life[i].sprite = NoHeart;
        }

        for (int i = 0; i<maxhealth; i++)
        {
            if (i<curhealth)
            {
                Life[i].sprite = Heart;
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
            jumpslidebutton.sprite = jump;
        }
        else
        {
            jumpslidebutton.sprite = slide;
        }
    }

    public void GameStart()
    {
        mainpanel.SetActive(false);
        howtoplaypanel.SetActive(true);
    }

    public void InGame()
    {
        howtoplaypanel.SetActive(false);
        ingamepanel.SetActive(true);

        curhealth = player.maxHealth;
    }

    public void GameOver()
    {
        ingamepanel.SetActive(false);
        gameoverpanel.SetActive(true);

        curscoreText.text = scoreText.text;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
