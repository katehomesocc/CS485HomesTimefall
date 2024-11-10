using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string currentScene = "Title";

    public GameObject settingsPanel;

    public bool isSettingsOpen = false;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
            DontDestroyOnLoad(transform.gameObject);
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key was pressed");

        }
        
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        this.currentScene = "Title";
    }

    public void LoadBattleScene()
    {
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
        this.currentScene = "Battle";
    }

    public void LoadAcademyScene()
    {
        SceneManager.LoadScene("AcademyScene", LoadSceneMode.Single);
        this.currentScene = "Academy";
    }

    public void LoadResearchScene()
    {
        SceneManager.LoadScene("ResearchScene", LoadSceneMode.Single);
        this.currentScene = "Research";
    }

    public void ToggleBattleSettingsMenu()
    {
        if(isSettingsOpen)
        {
            settingsPanel.SetActive(false);
            isSettingsOpen = false;
        } else {
            settingsPanel.SetActive(true);
            isSettingsOpen = true;
        }
        
    }

    public void QuitApplication()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
