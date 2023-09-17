using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private int sceneId = 1;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(20.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
    }
}
