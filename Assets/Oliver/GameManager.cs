using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    public SceneChange SceneChanger { get; private set; }
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(Instance != null){
            Debug.Log("Destroyed GameManager Instance");
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("awake");

    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        SceneChanger = GetComponent<SceneChange>();

        SceneManager.activeSceneChanged += OnSceneChange;
        Debug.Log("start");
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void OnSceneChange(Scene sc, Scene sc1) {
        Debug.Log("on scene change");
        SceneChanger = GetComponent<SceneChange>();
    }


}
