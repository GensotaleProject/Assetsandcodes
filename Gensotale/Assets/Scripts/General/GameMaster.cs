using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gameMaster;
    public static InputScript inputScript;

    static System.Random random = new System.Random(0);
    [HideInInspector] public int randomSeed;

    [HideInInspector] public float frameTime = 1f / 60f;

    public float timeScaleUpdate = 1;
    [HideInInspector] public float timeScale = 1;
    public bool lockMovement;

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
            inputScript = GetComponentInChildren<InputScript>();
            Application.targetFrameRate = 60;
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
        timeScale = timeScaleUpdate;
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




    //All random functions
    public void GenerateRandomSeed()
    {
        randomSeed = Mathf.RoundToInt((Input.mousePosition.x * 153) + (Input.mousePosition.y * 153) + Mathf.Clamp(Time.realtimeSinceStartup, 25, 158976000) +
            ((Time.unscaledDeltaTime * 10000) * (((inputScript.shootDown) ? 5 : 1) + ((inputScript.bombDown) ? 17 : -5) + ((inputScript.directionalInput.x > 0) ? 1 : 2)
            + ((inputScript.directionalInput.y < 0) ? 11 : 22))));
        UpdateSeed(randomSeed);
    }

    public void UpdateSeed(int newSeed)
    {
        randomSeed = newSeed;
        random = new System.Random(randomSeed);
        Debug.Log("Game's Random is set to " + randomSeed);
    }

    public static int Random(int lower, int upper)
    {
        return random.Next(lower, upper);
    }

    public static float Random(float lower, float upper)
    {

        return (((float)random.NextDouble() * Mathf.Abs(lower - upper)) + lower);
    }

    public static float Random(Vector2 constraints)
    {

        return (((float)random.NextDouble() * Mathf.Abs(constraints.x - constraints.y)) + constraints.x);
    }
}
