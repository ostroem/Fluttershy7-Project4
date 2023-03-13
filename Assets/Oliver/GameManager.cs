using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    public SceneChange SceneChanger { get; private set; }
    private Player player;
    [SerializeField] protected GameObject cinemaCamera;
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
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        SceneChanger = GetComponent<SceneChange>();
        player = FindObjectOfType<Player>();
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    public void RestartGame() {
        Destroy(player.gameObject);
        Destroy(gameObject);
        Destroy(cinemaCamera);
        SceneChange.currentActiveSceneIndex = 0;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy(){
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void OnSceneChange(Scene sc, Scene sc1) {
        SceneChanger = GetComponent<SceneChange>();
        player.increase_sceneNumber();
    }


}
