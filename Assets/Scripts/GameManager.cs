using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UltimateClean;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TMP_InputField playerNameInputField;
    public GameObject notificationPrefab;
    [SerializeField] private GameObject loadingPrefab;
    [SerializeField] private GameObject settingButton;
    [SerializeField] private PopupOpener offlinePopupOpener;

    public float money { get; set; }
    public int energy { get; set; }
    public float energyIncreaseRate { get; set; }
    public int crystal { get; set; }
    public int score { get; set; }
    public int tempScore { get; set; }
    public int level { get; set; }
    public string playerName { get; set; }
    public bool isSaved { get; set; }
    public Dictionary<string, int> offlineAwards { get; set; } // ("Energy", 100), ("Money", 1000)
    public TimeSpan offlineInterval { get; set; }

    bool newGame = true;
    public Canvas canvas { get; set; }
    public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.UnloadSceneAsync("LoadingScene");
            isSaved = false;
            newGame = true;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 300;
    }

    // Start is called before the first frame update
    void Init()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        money = ES3.Load<float>("money", "Player/Properties", 500000f);
        energy = ES3.Load<int>("energy", "Player/Properties", 1000);
        energyIncreaseRate = ES3.Load<float>("energyIncreaseRate", "Player/Properties", 1);
        crystal = ES3.Load<int>("crystal", "Player/Properties", 100);
        playerName = ES3.Load<string>("playerName", "Player/Name", "Energy Hunter");
        newGame = ES3.Load<bool>("newGame", "Game", true);
        playerNameInputField.text = playerName;
        score = ES3.Load<int>("score", "Player/Score", 0);
        level = 0;
        UpdateLevel();
    }

    void Start()
    {
        if (!newGame)
        {
            offlineInterval = DateTime.Now - TimeLine.Instance.lastQuitTime;
            float timeInterval = (float)offlineInterval.TotalSeconds;
            int count = (int)(timeInterval / TimeLine.Instance.updateFrequency);
            float moneyIncrease = count * MoneyBarCalculator.Instance.moneyAmount;
            int energyIncrease = count * EnergyBarCalculator.Instance.energy;
            offlineAwards = new Dictionary<string, int>();
            offlineAwards.Add("Money", (int)moneyIncrease);
            offlineAwards.Add("Energy", energyIncrease);
            offlinePopupOpener.OpenPopup();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLevel();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Camera.main.gameObject.name == "GameUI Camera")
                settingButton.GetComponent<ButtonWithSound>().onClick.Invoke();
        }
    }


    /// <summary>
    /// Add / Reduce money (success: return true, fail: return false)
    /// </summary>
    /// <param name="amount"></param>
    public bool AddMoney(float amount, bool showFailMessage = true)
    {
        if (amount + money < 0)
        {
            if (showFailMessage)
            {
                GameObject notification = Instantiate(notificationPrefab, canvas.transform);
                notification.GetComponent<Notification>().Launch(NotificationType.Pop, NotificationPositionType.TopMiddle, 1.5f, "Failed to pay", "You don't have enough money!");
            }
            return false;
        }
        money += amount;
        // Debug.Log("Money: " + money);
        return true;
    }

    /// <summary>
    /// Add / Reduce energy (success: return true, fail: return false)
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool AddEnergy(int amount, bool showFailMessage = true)
    {
        if (amount + energy < 0)
        {

            if (showFailMessage)
            {
                GameObject notification = Instantiate(notificationPrefab, canvas.transform);
                notification.GetComponent<Notification>().Launch(NotificationType.Pop, NotificationPositionType.TopMiddle, 1.5f, "Failed to pay", "You don't have enough energy!");
            }
            return false;
        }
        energy += amount;
        return true;
    }



    /// <summary>
    /// Calculates the level and level progress of the player
    /// </summary>
    /// <returns></returns>
    public void UpdateLevel()
    {
        tempScore = LevelManager.level + score;
        if (tempScore >= Settings.totalLevel[level])
        {
            for (int i = 0; i <= level; i++)
            {
                if (tempScore >= Settings.totalLevel[i])
                    tempScore = tempScore - Settings.totalLevel[i];
            }
            for (int i = level + 1; i < Settings.totalLevel.Count; i++)
            {
                if (tempScore < Settings.totalLevel[i])
                {
                    level = i;
                    tempScore = tempScore - Settings.totalLevel[i - 1];
                    break;
                }

            }
        }

    }

    private void PlayerInformationSave()
    {
        playerName = playerNameInputField.text;
        ES3.Save<float>("money", money, "Player/Properties");
        ES3.Save<int>("energy", energy, "Player/Properties");
        ES3.Save<float>("energyIncreaseRate", energyIncreaseRate, "Player/Properties");
        ES3.Save<string>("playerName", playerName, "Player/Name");
        ES3.Save<bool>("newGame", false, "Game");
        ES3.Save<int>("score", score, "Player/Score");
    }

    /// <summary>
    /// Integrate all the saving processes of the game
    /// </summary>
    public IEnumerator Save()
    {
        Debug.Log("Saving data");
        StartLoadingUI();
        PlayerInformationSave(); // save player information
        yield return null;
        BuildingManager.Instance.Save(); // save buildings information
        yield return null;
        ShopManager.Instance.SaveShopItem(); // save shop items information
        yield return null;
        // store level factors
        LevelManager.getAllLevelFactors().ForEach(levelFactor => levelFactor.store());
        yield return null;
        // store semantic data
        BuildingLoader.Instance.store();
        yield return null;
        DataSetting.store();
        yield return null;
        // save current time
        TimeLine.Instance.SaveTimeStamp();
        yield return null;
        ShopInstallManager.Instance.SaveShopItems(); // save single shop items installation
        yield return null;
        SoundManager.Instance.Save(); // save sound settings
        yield return null;
        isSaved = true;
    }

    /// <summary>
    /// API: Create loading UI
    /// </summary>
    public void StartLoadingUI()
    {
        AddBackground();
        Instantiate(loadingPrefab, canvas.transform);
    }

    private void AddBackground()
    {
        var bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, backgroundColor);
        bgTex.Apply();

        var m_background = new GameObject("PopupBackground");
        var image = m_background.AddComponent<Image>();
        var rect = new Rect(0, 0, bgTex.width, bgTex.height);
        var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
        image.material.mainTexture = bgTex;
        image.sprite = sprite;
        var newColor = image.color;
        image.color = newColor;
        image.canvasRenderer.SetAlpha(0.0f);
        image.CrossFadeAlpha(1.0f, 0.4f, false);

        m_background.transform.localScale = new Vector3(1, 1, 1);
        m_background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        m_background.transform.SetParent(canvas.transform, false);
        m_background.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }


    void OnDestroy()
    {
        DataSetting.store(); // avoid data loss causing problems
        ShopManager.Instance.SaveShopItem(); // save shop items information
    }
}
