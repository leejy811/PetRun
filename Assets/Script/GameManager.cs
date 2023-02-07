using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //BackGround, Map 관련
    private int curMapIndex;
    private int curBackIndex;
    private readonly float backGroundSize = 23.5f;
    private readonly float mapLength = 30;
    private string[] mapType = { "Idle", "Jump", "Slide" };

    //플래그 변수
    public bool isStart;
    private bool isReady;

    //오브젝트, 컴포넌트 변수
    public Player player;
    [SerializeField] Transform[] backGround;
    [SerializeField] GameObject[] mapObject;
    [SerializeField] CountDown countDown;
    PoolManager poolManager;

    //효과음 클립
    [SerializeField] AudioClip highScoreSound;
    [SerializeField] AudioClip gameOverSound;
    [SerializeField] AudioClip startSound;
    [SerializeField] AudioClip goSound;

    //BGM 클립
    [SerializeField] AudioClip lobbyIntroSound;
    [SerializeField] AudioClip lobbyLoopSound;
    [SerializeField] AudioClip onGameIntroSound;
    [SerializeField] AudioClip onGameLoopSound;

    //오디오 소스
    [SerializeField] AudioSource effectSoundSource;
    [SerializeField] AudioSource bgmSoundSource;
    [SerializeField] AudioSource startSoundSource;
    [SerializeField] AudioSource gunSoundSource;

    //아이템 생성 위치 관련
    [SerializeField] Transform[] jumpItemSpawnPos;
    [SerializeField] Transform[] SlideItemSpawnPos;

    void Awake()
    {
        SetResolution();
        poolManager = GetComponent<PoolManager>();
    }

    public void SetResolution()
    {
        int setWidth = 1920;
        int setHeight = 1080;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight)
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight);
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); 
        }
        else
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight);
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }

    void Update()
    {
        MapCheck();
        BackCheck();
        SoundCheck();
        QuitCheck();
    }

    void MapCheck() { 
        float distance = player.transform.position.x - mapObject[curMapIndex].transform.position.x;

        if (Mathf.Abs(distance) < 11)
        {
            int ranIndex = Random.Range(0, 100);
            int ranMapType;

            if (ranIndex >= 90)
                ranMapType = 0;
            else if (ranIndex >= 45)
                ranMapType = 1;
            else
                ranMapType = 2;

            PlaceMap();
            PlaceObstcle(ranMapType);
            PlaceItem(ranMapType);
        }
    }

    void PlaceMap()
    {
        curMapIndex = curMapIndex == 0 ? 1 : 0;

        mapObject[curMapIndex].transform.position = curMapIndex == 0 ? new Vector3(mapObject[1].transform.position.x + mapLength, -0.5f, 0) : new Vector3(mapObject[0].transform.position.x + mapLength, -0.5f, 0);
    }

    void PlaceObstcle(int mapTypeIndex)
    {
        if (!isStart || mapType[mapTypeIndex] == "Idle")
            return;

        Obstacle newObstacle;

        if (mapType[mapTypeIndex] == "Jump")
            newObstacle = poolManager.GetFromPool<Obstacle>("Jump");
        else
            newObstacle = poolManager.GetFromPool<Obstacle>("Slide");

        newObstacle.transform.position = new Vector3(mapObject[curMapIndex].transform.position.x, newObstacle.placePos.y, -1);
        newObstacle.gameManager = this;
    }

    void PlaceItem(int mapTypeIndex)
    {
        if (!isStart)
            return;

        if (mapType[mapTypeIndex] == "Jump")
            foreach (Transform spawnPos in jumpItemSpawnPos)
                SetItemPosition(spawnPos);
        else if (mapType[mapTypeIndex] == "Slide")
            foreach (Transform spawnPos in SlideItemSpawnPos)
                SetItemPosition(spawnPos);
        else if (mapType[mapTypeIndex] == "Idle")
        {
            int ranType = Random.Range(0, 2);
            if (ranType == 0)
                foreach (Transform spawnPos in jumpItemSpawnPos)
                    SetItemPosition(spawnPos);
            else
                foreach (Transform spawnPos in SlideItemSpawnPos)
                    SetItemPosition(spawnPos);
        }
    }

    void SetItemPosition(Transform spawnPos)
    {
        Item newItem = Spawn();
        if (newItem != null)
        {
            newItem.transform.position = spawnPos.position + mapObject[curMapIndex].transform.position;
            newItem.Init();
            newItem.manager = this;
        }
    }

    void BackCheck()
    {
        float distance = backGround[curBackIndex].position.x - player.transform.position.x;

        if (distance < 6)
        {
            curBackIndex = curBackIndex == 0 ? 1 : 0;
            backGround[curBackIndex].position = curBackIndex == 0 ? new Vector3(backGround[1].position.x + backGroundSize, 1.5f, 10) : new Vector3(backGround[0].position.x + backGroundSize, 1.5f, 10);
        }
    }

    void SoundCheck()
    {
        if (isReady || player.isFall)
        {
            bgmSoundSource.Stop();
            return;
        }

        if (bgmSoundSource.isPlaying == false)
        {
            if (!isStart)
            {
                PlaySound("LobbyLoop");
                bgmSoundSource.loop = true;
            }
            else
            {
                PlaySound("OnGameLoop");
                bgmSoundSource.loop = true;
            }
        }
    }

    void QuitCheck()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    Item Spawn()
    {
        int ran = Random.Range(0, 100);

        if (ran > 20)
        {
            return null;
        }
        else if (ran > 10)
        {
            Item newItem = poolManager.GetFromPool<Item>("Bone");
            return newItem;
        }
        else
        {
            Item newItem = poolManager.GetFromPool<Item>("Chur");
            return newItem;
        }
    }

    public void ReturnPool(Item clone)
    {
        poolManager.TakeToPool<Item>(clone.itemType, clone);
        clone.isEnable = false;
    }

    public void ReturnPool(Obstacle clone)
    {
        poolManager.TakeToPool<Obstacle>(clone.obstacleType, clone);
        clone.isEnable = false;
    }

    public void PlaySound(string soundType)
    {
        AudioSource audioSource = null;

        switch (soundType)
        {
            case "HighScore":
            case "GameOver":
            case "Start":
            case "Go":
                audioSource = effectSoundSource;
                break;
            case "LobbyIntro":
            case "LobbyLoop":
            case "OnGameIntro":
            case "OnGameLoop":
                audioSource = bgmSoundSource;
                break;
        }

        switch (soundType)
        {
            case "HighScore":
                audioSource.clip = highScoreSound;
                break;
            case "GameOver":
                audioSource.clip = gameOverSound;
                break;
            case "Start":
                audioSource.clip = startSound;
                break;
            case "Go":
                audioSource.clip = goSound;
                break;
            case "LobbyIntro":
                audioSource.clip = lobbyIntroSound;
                break;
            case "LobbyLoop":
                audioSource.clip = lobbyLoopSound;
                break;
            case "OnGameIntro":
                audioSource.clip = onGameIntroSound;
                audioSource.loop = false;
                break;
            case "OnGameLoop":
                audioSource.clip = onGameLoopSound;
                break;
        }

        audioSource.Play();
    }

    public IEnumerator StartSet()
    {
        PlaySound("Go");
        isReady = true;
        countDown.transform.position = new Vector3(player.transform.position.x + 4, -2, 9);
        countDown.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        startSoundSource.Play();
        yield return new WaitForSeconds(3.0f);
        gunSoundSource.Play();
        yield return new WaitForSeconds(1.0f);
        PlaySound("OnGameIntro");
        isReady = false;
        isStart = true;
    }
}
