using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class TextGuesser : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField inputField;

    private Action<bool> onSubmit;
    private string checkString;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Match();
        }
    }

    private void Match()
    {
        var match = checkString == inputField.text;
        onSubmit?.Invoke(match);
    }
}
