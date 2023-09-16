using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class TextGuesser : MonoBehaviour
{
    private const string GIBBERISH = "SOME GIBBERISH TO ALWAYS FAIL TEXT CHECK";
    [SerializeField] private TMPro.TMP_InputField inputField;
    [SerializeField] private GameObject acceptBttn;
    private Action onSubmit;
    private string checkString;
    private Animator animator;
    
    public bool IsOpen { get; private set; }

    private void Start()
    {
        animator = GetComponent<Animator>();
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
        inputField.Select();
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
        var chk = Validate(checkString);
        var str = Validate(inputField.text);
        if (str == chk)
        {
            Debug.Log("SUCC");
            onSubmit?.Invoke();
        }
        else
        {
            Debug.Log("FAIL");
            animator.SetTrigger("FAIL");
            StartCoroutine(SelectAfter());
        }
    }

    IEnumerator SelectAfter()
    {
        yield return new WaitForSeconds(0.2f);
        inputField.Select();
    }
    private string Validate(string input)
    {
        return input.Trim().ToLower();
    }
}
