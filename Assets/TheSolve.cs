using TMPro;
using UnityEngine;

public class TheSolve : MonoBehaviour
{
    public TMP_Dropdown MurdererDropdown;
    public TMP_Dropdown AlibiADropdown;
    public TMP_Dropdown AlibiBDropdown;

    public void Solve()
    {
        if (MurdererDropdown.value == 4 && 
            ((AlibiADropdown.value == 2 && AlibiBDropdown.value == 3) || 
             (AlibiADropdown.value == 3 && AlibiBDropdown.value == 2)))
        {
            Debug.Log("Solved");
        }
        else
        {
            Debug.LogError("Do better BC");
        }
    }
}