using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private int sceneId = 1;
    [SerializeField] private GameObject skipCanvas;
    private IEnumerator Start()
    {
        bool canSkip = false;
        if (PlayerPrefs.HasKey("CanSkip"))
        {
            skipCanvas.gameObject.SetActive(true);
            canSkip = true;
        }

        float timeStep = 0;
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime / 20.5f;
            if (canSkip && Input.GetKey(KeyCode.Return))
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
        PlayerPrefs.SetInt("CanSkip",0);
        PlayerPrefs.Save();
    }
}
