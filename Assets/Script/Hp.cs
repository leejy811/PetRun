using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{
    //Life 집합
    public Image[] Life;

    public int HP { get; private set; }

    private int MaxHp;

    public Sprite Heart, NoHeart;

    private void Start()
    {
        //최대 체력 정의
        MaxHp= Life.Length;
        //HP 초기화
        HP = MaxHp;
        
        for(int i=0; i<MaxHp; i++)
        {
            if(HP>i)
            {
                Life[i].sprite = Heart;
            }
        }
    }

    public void UpDownHp(int v)
    {
        //체력 증가or감소
        HP += v;

        //HP는 0이상 MaxHp 이하
        HP = Mathf.Clamp(HP, 0, MaxHp);

        for(int i=0; i<MaxHp; i++)
        {
            Life[i].sprite = NoHeart;
        }

        for(int i=0; i<MaxHp; i++)
        {
            if(HP>i)
            {
                Life[i].sprite = Heart;
            }
        }
    }

    public void DownHp()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
