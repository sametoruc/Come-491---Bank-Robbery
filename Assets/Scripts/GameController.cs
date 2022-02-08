using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool hasKey, isLevelCompleted;
    
    private void Awake()
	{
		if (!instance)
		{
            instance = this;
		}
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WinLevel()
    {
        isLevelCompleted = true;
        FinalDoor.instance.ConfettisOn();
        UIController.instance.WinScreen();
    }
    public void FailLevel()
    {
        isLevelCompleted = true;
        UIController.instance.LoseScreen();
    }
}
