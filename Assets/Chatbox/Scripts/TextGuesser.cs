using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class TextGuesser : MonoBehaviour
{
    private const string GIBBERISH = "SOME GIBBERISH TO ALWAYS FAIL TEXT CHECK";
    [SerializeField] private TMPro.TMP_InputField inputField;
    [SerializeField] private GameObject acceptBttn;
    private Action onSubmit;
    private string checkString;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        inputField.onSubmit.AddListener((str) =>  Match());
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(inputField.text) || string.IsNullOrWhiteSpace(inputField.text))
        {
            if(acceptBttn.activeSelf)
                acceptBttn.SetActive(false);
        }
        else
        {
            if(!acceptBttn.activeSelf)
                acceptBttn.SetActive(true);
        }
    }

    public void Open(string checkString, Action onSubmit)
    {
        this.checkString = checkString;
        this.onSubmit = onSubmit;
        IsOpen = true;
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        onSubmit = null;
        checkString = GIBBERISH;
        IsOpen = false;
        this.gameObject.SetActive(false);
    }
    
    private void Match()
    {
        Debug.Log("Matching...");
        checkString = Validate(checkString);
        var str = Validate(inputField.text);
        Debug.Log($"{str} -- {checkString}");
        if (str == checkString)
        {
            Debug.Log("SUCC");
            onSubmit?.Invoke();
        }
        else
        {
            Debug.Log("FAIL");
            //Shake();
        }
    }

    private string Validate(string input)
    {
        var str = input.Replace(",", "");
        str = str.Replace(".", "");
        str = str.Replace(";", "");
        str = str.Replace("?", "");
        str = str.Replace("?", "\"");
        str = str.Trim();
        return str.ToLower();
    }
}
