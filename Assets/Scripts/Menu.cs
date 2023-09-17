using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnClickPlay()
    {
        AudioManager.Instance.PlaySFX(AudioManager.ClipTypes.Click);
        SceneManager.LoadScene(02);
    }
}
