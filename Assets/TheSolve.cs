using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TheSolve : MonoBehaviour
{
    public TMP_Dropdown MurdererDropdown;
    public TMP_Dropdown AlibiADropdown;
    public TMP_Dropdown AlibiBDropdown;

    public GameObject rightPanel;
    public GameObject wrongPanel;

    public void Solve()
    {
        if (MurdererDropdown.value == 4 && 
            ((AlibiADropdown.value == 2 && AlibiBDropdown.value == 3) || 
             (AlibiADropdown.value == 3 && AlibiBDropdown.value == 2)))
        {
            rightPanel.gameObject.SetActive(true);
            rightPanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Menu");
            });
        }
        else
        {
            wrongPanel.gameObject.SetActive(true);
        }
    }
}