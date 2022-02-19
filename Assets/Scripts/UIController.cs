using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI LevelText;
    public Sprite[] WinEmojis, LoseEmojis;
    public Image WinEmoji, LoseEmoji;
    public GameObject WinScreenPanel, LoseScreenPanel, LevelSelectPanel, LevelButton, InGameButtonsContainer;
    public Transform LevelButtonContainer;
    public int LevelCount, LevelID;
    public static UIController instance;
    public static bool isFirstSesion;
    public static int testValue;
    private void Awake()
    {
        if (!instance)
            instance = this;
    }
    void Start()
    {        
        if(PlayerPrefs.GetInt("isFirstRun") == 0 && true)
        {
            PlayerPrefs.SetInt("isFirstRun", 1);
            PlayerPrefs.SetInt("Level", 1); //Level geçildi mi 0-1
        }
        if (!isFirstSesion)
        {
            isFirstSesion = true;
           // SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
        }
        LevelText.text = "LEVEL " + (SceneManager.GetActiveScene().buildIndex).ToString();
        CreateLevelButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            PlayerPrefs.DeleteAll();
        }
    }
    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {               
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex == PlayerPrefs.GetInt("Level"))
            {
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }
    public void WinScreen()
    {
        LevelSelectPanel.SetActive(false);
        InGameButtonsContainer.SetActive(false);
        WinScreenPanel.SetActive(true);
        WinEmoji.sprite = WinEmojis[Random.Range(0, WinEmojis.Length)];
    }

    public void LoseScreen()
    {
        LevelSelectPanel.SetActive(false);
        InGameButtonsContainer.SetActive(false);
        LoseScreenPanel.SetActive(true);
        LoseEmoji.sprite = LoseEmojis[Random.Range(0, LoseEmojis.Length)];
    }
    public void LevelSelectScreen()
    {
        LevelSelectPanel.SetActive(true);
    }
    public void CloseLevelSelectScreen()
    {
        LevelSelectPanel.SetActive(false);
    }
    public void CreateLevelButtons()
    {
        LevelCount = SceneManager.sceneCountInBuildSettings;
        for(int i = 0; i<LevelCount-1; i++)
        {
            GameObject tempButton = Instantiate(LevelButton, LevelButtonContainer);
            tempButton.GetComponent<LevelButton>().InitializeButton(i+1);

            tempButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(-1400 + 250*((i / 9) * 3 + (i % 9) / 3), 250 - 250*( (i % 3)), 0);
        }
    }
    public void SelectLevel(int levelNo)
    {
        SceneManager.LoadScene(levelNo);
    }
}
