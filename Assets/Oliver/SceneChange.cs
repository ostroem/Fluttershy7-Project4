using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    static int currentActiveSceneIndex = 0;
    
    public void ChangeScene() {
        if(currentActiveSceneIndex >= SceneManager.sceneCountInBuildSettings - 1){
            return;
        }
        SceneManager.LoadScene(++currentActiveSceneIndex);
    }


}
