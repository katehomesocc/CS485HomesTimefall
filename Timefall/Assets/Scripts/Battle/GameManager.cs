using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static string TITLE_SCENE = "TitleScene";
    public static string ACADEMY_SCENE = "AcademyScene";
    public static string BATTLE_SCENE = "BattleScene";
    public static string RESEARCH_SCENE = "ResearchScene";
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
        Debug.Log("OnSceneLoaded: " + scene.name);
        this.currentScene = scene.name;

        StartCoroutine(OpenDoors());
        
    }

    IEnumerator LoadNextScene(string sceneName)
    {
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

    public void LoadTitleScene()
    {
        StartCoroutine(LoadNextScene(TITLE_SCENE));
    }

    public void LoadBattleScene()
    {
        StartCoroutine(LoadNextScene(BATTLE_SCENE));
    }

    public void LoadAcademyScene()
    {
        
        // StartCoroutine(LoadNextScene(ACADEMY_SCENE));
    }

    public void LoadResearchScene()
    {
        StartCoroutine(LoadNextScene(RESEARCH_SCENE));
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
