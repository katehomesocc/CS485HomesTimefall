using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public const string TITLE_SCENE = "TitleScene";
    public const string ACADEMY_SCENE = "AcademyScene";
    public const string BATTLE_SCENE = "BattleScene";
    public const string RESEARCH_SCENE = "ResearchScene";
    public string currentScene = "Title";

    public GameObject settingsPanel;

    public bool isSettingsOpen = false;
    
    [Header("Scene Transitions")]
    public RawImage researchDoorLeft;
    public RawImage researchDoorRight;
    public float doorClosingTime = .75f;
    public bool doorIsOpen = true;
    public Slider sceneLoadingBar;

    [Header("Door Textures")]
    public Texture DOOR_LEFT_OPEN_TEX;
    public Texture DOOR_LEFT_CLOSED_TEX;
    public Texture DOOR_RIGHT_OPEN_TEX;
    public Texture DOOR_RIGHT_CLOSED_TEX;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
            return;
        } 
        else 
        { 
            Instance = this; 
            DontDestroyOnLoad(transform.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        } 
    }

    // called third
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debug.Log("OnSceneLoaded: " + scene.name);
        this.currentScene = scene.name;

        switch (currentScene)
        {
            case TITLE_SCENE:
                AudioManager.Instance.PlayTitleMusic();
                break;
            case BATTLE_SCENE:
                AudioManager.Instance.PlayBattleMusic();
                break;
            case RESEARCH_SCENE:
                AudioManager.Instance.PlayResearchMusic();
                break;
            default:
                break;
        }

        StartCoroutine(OpenDoors());
        
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadNextScene(sceneName));
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        AudioManager.Instance.PlayTransitionMusic();
        yield return StartCoroutine(CloseDoors());

        sceneLoadingBar.gameObject.SetActive(true);
        sceneLoadingBar.value = 1f;

        Debug.Log(string.Format("Async loading [{0}]", sceneName));
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);

        while(!sceneLoad.isDone)
        {
            float progress = Mathf.Clamp01(.9f - (sceneLoad.progress / .9f));
            // Debug.Log(string.Format("progress [{0}]", progress));
            sceneLoadingBar.value = progress;
            yield return null;
        }

        sceneLoadingBar.gameObject.SetActive(false);
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

    public static void LoadTitleScene()
    {
        Instance.LoadScene(TITLE_SCENE);
    }

    public static void LoadBattleScene()
    {
        Instance.LoadScene(BATTLE_SCENE);
    }

    public static void LoadAcademyScene()
    {
        
        // StartCoroutine(Instance.LoadNextScene(ACADEMY_SCENE));
    }

    public static void LoadResearchScene()
    {
        Instance.LoadScene(RESEARCH_SCENE);
    }

    public void ToggleSettingsMenu()
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

    public IEnumerator OpenDoors()
    {
        if(doorIsOpen) {yield break;}
        Debug.Log("Opening research doors");

        researchDoorLeft.texture = DOOR_LEFT_OPEN_TEX;
        researchDoorRight.texture = DOOR_RIGHT_OPEN_TEX;

        Vector3 leftPos = researchDoorLeft.transform.position;
        Vector3 rightPos = researchDoorRight.transform.position;

        Vector3 openLeftPos = new Vector3(leftPos.x - 960, leftPos.y, leftPos.z);
        Vector3 openRightPos = new Vector3(rightPos.x + 960, rightPos.y, rightPos.z);

        StartCoroutine(MoveToPosition(researchDoorLeft.transform, openLeftPos, doorClosingTime));
        yield return StartCoroutine(MoveToPosition(researchDoorRight.transform, openRightPos, doorClosingTime));

        doorIsOpen = true;
    }

    public IEnumerator CloseDoors()
    {
        if(!doorIsOpen) {yield break;}
        Debug.Log("Closing research doors");

        Vector3 leftPos = researchDoorLeft.transform.position;
        Vector3 rightPos = researchDoorRight.transform.position;
        
        Vector3 closedLeftPos = new Vector3(leftPos.x + 960, leftPos.y, leftPos.z);
        Vector3 closedRightPos = new Vector3(rightPos.x - 960, rightPos.y, rightPos.z);

        StartCoroutine(MoveToPosition(researchDoorLeft.transform, closedLeftPos, doorClosingTime));
        yield return StartCoroutine(MoveToPosition(researchDoorRight.transform, closedRightPos, doorClosingTime));

        researchDoorLeft.texture = DOOR_LEFT_CLOSED_TEX;
        researchDoorRight.texture = DOOR_RIGHT_CLOSED_TEX;
        doorIsOpen = false;
    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while(t <= 1f)
        {
                t += Time.deltaTime / timeToMove;
                transform.position = Vector3.Lerp(currentPos, position, t);
                yield return null;
        }
        transform.position = position;
    }

    public void UpdateMasterVolume(float volume)
	{
		AudioManager.Instance.UpdateMasterVolume(volume);
        PlayerPrefs.SetFloat("masterVolume", volume);
	}

	public void UpdateEffectVolume(float volume)
	{
		AudioManager.Instance.UpdateEffectVolume(volume);
        PlayerPrefs.SetFloat("effectVolume", volume);
	}

	public void UpdateMusicVolume(float volume)
	{
		AudioManager.Instance.UpdateMusicVolume(volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
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
