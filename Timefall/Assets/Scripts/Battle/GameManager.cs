using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string currentScene = "Title";

    public GameObject settingsPanel;

    public bool isSettingsOpen = false;

    [Header("Scene Transitions")]
    public Transform researchDoorLeft;
    public Transform researchDoorRight;
    public float doorClosingTime = .75f;
    public bool doorIsOpen = true;
    public Slider sceneLoadingBar;

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
        } 

        sceneLoadingBar = GameObject.FindWithTag("LoadingBar").GetComponent<Slider>();

        sceneLoadingBar.gameObject.SetActive(false);

        researchDoorLeft = GameObject.FindWithTag("Door left").transform;
        researchDoorRight = GameObject.FindWithTag("Door right").transform;

        if(currentScene == "Research")
        {
            OpenResearchDoors();
        }
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        yield return StartCoroutine(CloseResearchDoors());

        sceneLoadingBar.gameObject.SetActive(true);

        Debug.Log(string.Format("Async loading [{0}]", sceneName));
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);

        while(!sceneLoad.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoad.progress / .9f);
            sceneLoadingBar.value = progress;
            yield return null;
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
        
        // SceneManager.LoadScene("AcademyScene", LoadSceneMode.Single);
        // this.currentScene = "Academy";
    }

    public void LoadResearchScene()
    {
        StartCoroutine(LoadNextScene("ResearchScene"));
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

    public IEnumerator OpenResearchDoors()
    {
        if(doorIsOpen) {yield break;}
        Debug.Log("Opening research doors");

        Vector3 openLeftPos = new Vector3(researchDoorLeft.position.x - 960, researchDoorLeft.position.y, researchDoorLeft.position.z);
        Vector3 openRightPos = new Vector3(researchDoorRight.position.x + 960, researchDoorRight.position.y, researchDoorRight.position.z);

        StartCoroutine(MoveToPosition(researchDoorLeft, openLeftPos, doorClosingTime));
        yield return StartCoroutine(MoveToPosition(researchDoorRight, openRightPos, doorClosingTime));


    }

    public IEnumerator CloseResearchDoors()
    {
        if(!doorIsOpen) {yield break;}
        Debug.Log("Closing research doors");
        
        Vector3 closedLeftPos = new Vector3(researchDoorLeft.position.x + 960, researchDoorLeft.position.y, researchDoorLeft.position.z);
        Vector3 closedRightPos = new Vector3(researchDoorRight.position.x - 960, researchDoorRight.position.y, researchDoorRight.position.z);

        StartCoroutine(MoveToPosition(researchDoorLeft, closedLeftPos, doorClosingTime));
        yield return StartCoroutine(MoveToPosition(researchDoorRight, closedRightPos, doorClosingTime));

        // SceneManager.LoadScene("ResearchScene", LoadSceneMode.Single);
        // this.currentScene = "Research";
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
