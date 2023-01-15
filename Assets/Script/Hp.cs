using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{
    //Life ����
    public Image[] Life;

    public int HP { get; private set; }

    private int MaxHp;

    public Sprite Heart, NoHeart;

    private void Start()
    {
        //�ִ� ü�� ����
        MaxHp= Life.Length;
        //HP �ʱ�ȭ
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
        //ü�� ����or����
        HP += v;

        //HP�� 0�̻� MaxHp ����
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
