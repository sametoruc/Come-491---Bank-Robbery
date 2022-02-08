using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LevelButton : MonoBehaviour
{
    // Start is called before the first frame update
    public int LevelId;
    public TextMeshProUGUI LevelIdText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitializeButton(int levelNo)
    {
        LevelId = levelNo;
        LevelIdText.text = levelNo.ToString();
        if(LevelId > PlayerPrefs.GetInt("Level")) //geçilmiş bölümün butonu mu
        {
            GetComponent<Button>().interactable = false;
        }
    }
    public void ButtonClick()
    {
        UIController.instance.SelectLevel(LevelId);
    }
}
