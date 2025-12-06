using System;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text cubeCountText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
         TimerManager.OnTimerUpdated += UpdateTimerUI;
       PlayerControler.updateCubeCount += UpdateCubeCountUI;
        
    }
    void OnDisable()
    {
       TimerManager.OnTimerUpdated -= UpdateTimerUI;
       PlayerControler.updateCubeCount -= UpdateCubeCountUI;  
    }
    void Start()
    {
      
    }

    void UpdateTimerUI(string timeText)
    {
        timerText.text = timeText;
    }
    void UpdateCubeCountUI(string timeText)
    {
        cubeCountText.text = timeText;
    }

}
