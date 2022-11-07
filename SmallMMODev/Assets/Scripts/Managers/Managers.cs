using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_Instance;
    static Managers Instance 
    { 
        get 
        { 
            Init(); 
            return s_Instance; 
        } 
    }

    #region properties
    GameManagerEx _game = new GameManagerEx(); 
    public static GameManagerEx Game
    {
        get
        {
            return Instance._game;
        }
    }

    DataManager _dataManager = new DataManager();
    public static DataManager Data
    {
        get
        {
            return Instance._dataManager;
        }
    }

    InputManager _input = new InputManager();
    public static InputManager Input
    {
        get
        {
            return Instance._input;
        }
    }

    PoolManager _poolManager = new PoolManager();
    public static PoolManager Pool
    {
        get
        {
            return Instance._poolManager;
        }
    }

    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource
    {
        get
        {
            return Instance._resource;
        }
    }

    SceneManagerEx _sceneManager = new SceneManagerEx();
    public static SceneManagerEx SceneEX
    {
        get
        {
            return Instance._sceneManager;
        }
    }

    UIManager _uimanager = new UIManager();
    public static UIManager UIManager
    {
        get
        {
            return Instance._uimanager;
        }
    }

    SoundManager _soundManager = new SoundManager();
    public static SoundManager Sound
    {
        get
        {
            return Instance._soundManager;
        }
    }
    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if(s_Instance == null)
        {
            GameObject obj = GameObject.Find("@Managers");
            if(obj == null)
            {
                obj = new GameObject { name = "@Managers" };
                obj.AddComponent<Managers>();
            }
            DontDestroyOnLoad(obj);
            s_Instance = obj.GetComponent<Managers>();

            s_Instance._game.Init();

            s_Instance._dataManager.Init();
            s_Instance._poolManager.Init();
            s_Instance._soundManager.Init();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        SceneEX.Clear();
        Sound.Clear();
        UIManager.Clear();

        Pool.Clear();
    }
}
