using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gameMaster;

    [Header("Player Stats")]
    public int exp;
    public int lv;

    [Header("Object References")]
    public Transform playerTrans;

    [Header("Scene Transition")]
    public SceneType curSceneType;
    public bool loading;
    public Vector2 playerPosInPrevScene;
    [HideInInspector] public int enteringDoor = -1;

    public Language languageSetting;
    public enum SceneType { Overworld, Battle, Menu, Misc }
    public enum Language { English, 日本語, Español, Deutsche }

    private void Awake()
    {
        if (gameMaster == null)
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ReferenceSetup;
            gameMaster = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string sceneName, bool storePrevPos, int door, SceneType newSceneType)
    {
        if (loading)
            return;

        if (storePrevPos && curSceneType != SceneType.Menu && curSceneType != SceneType.Misc)
            playerPosInPrevScene = playerTrans.position;

        gameMaster.StartCoroutine(LoadingScene(sceneName));
        enteringDoor = door;
        loading = true;
    }


    IEnumerator LoadingScene(string sceneName)
    {
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            yield return null;
        }
    }

    public void ReferenceSetup(UnityEngine.SceneManagement.Scene previousScene, UnityEngine.SceneManagement.Scene newScene)
    {
        loading = false;
        switch (curSceneType)
        {
            case SceneType.Overworld:
                playerTrans = GameObject.Find("RenkoOverworld").transform;
                break;

            case SceneType.Battle:

                break;
        }
    }
}
