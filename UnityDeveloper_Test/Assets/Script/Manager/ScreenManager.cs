using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    
    public GameObject GameStartPanel;
    public GameObject GameOverPanel;
    public GameObject GameLevelCompleted;

    [SerializeField] Button playNow;
    [SerializeField] Button replayBtn;
    [SerializeField] Button replayBtn2;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnEnable()
    {
        TimerManager.OnTimerFinished += ShowGameOverScreen;
        TimerManager.OnTimerStart += DisableStartScreen;
        PlayerControler.showGameCompleted += ShowGameLevelScreen;
        playNow.onClick.AddListener(OnClickPlayBtn);
        replayBtn.onClick.AddListener(OnClickReplayBtn);
         replayBtn2.onClick.AddListener(OnClickReplayBtn);
    }

    
    void OnDisable()
    {
        TimerManager.OnTimerFinished -= ShowGameOverScreen;
        TimerManager.OnTimerStart -= DisableStartScreen;
        PlayerControler.showGameCompleted -= ShowGameLevelScreen;

         playNow.onClick.RemoveListener(OnClickPlayBtn);
        replayBtn.onClick.RemoveListener(OnClickReplayBtn);
        replayBtn2.onClick.RemoveListener(OnClickReplayBtn);
    }
    void Start()
    {
        GameStartPanel.SetActive(true);  
        GameOverPanel.SetActive(false);  
        GameLevelCompleted.SetActive(false);
    }

    void DisableStartScreen()
    {
      GameStartPanel.SetActive(false);  
    }
    void ShowGameOverScreen()
    {
        GameOverPanel.SetActive(true);
        DisableStartScreen();
    }

    public void ShowGameLevelScreen()
    {
        GameOverPanel.SetActive(true);
        DisableStartScreen();
        TimerManager.instance.StopTimer();
    }

    public void OnClickPlayBtn()
    {
        TimerManager.instance.OnClikcStartTimer();
    }
    public void OnClickReplayBtn()
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   
}
