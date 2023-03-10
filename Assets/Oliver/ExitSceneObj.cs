using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSceneObj : MonoBehaviour
{
    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>

    [SerializeField] Vector2 exitPosition;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            GameManager.Instance.SceneChanger.ChangeScene();
            other.transform.position = exitPosition;
        }
    }


}
