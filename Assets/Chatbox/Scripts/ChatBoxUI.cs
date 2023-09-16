using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TextAsset = UnityEngine.TextAsset;

public class ChatBoxUI : MonoBehaviour
{
    public enum LanguageMode
    {
        Secret,
        English
    }

    private static ChatBoxUI instance;

    public static ChatBoxUI Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ChatBoxUI>();

            return instance;
        }
    }
    
    public bool IsOpen { get; private set; }

    private string CHAT_PATH = "ChatData";
    private string CHAR_IMG_PATH = "Characters";
    [SerializeField] private CanvasGroup cg;
    
    [SerializeField] private GameObject chatBoxArea;
    [SerializeField] private TMP_FontAsset secretFontAsset;
    [SerializeField] private TMP_FontAsset discoveredFontAsset;
    [SerializeField] private TMP_Text chatTextBox;
    [SerializeField] private TMP_Text nameTextBox;
    [SerializeField] private TextGuesser textGuesser;
    [SerializeField] private string continueCode;
    [SerializeField] private string HiddenSymbol = "[]";
    [SerializeField] private List<Image> availbleSprites;
    [SerializeField] private List<char> basicUnlockedSymbols;
    [SerializeField] private List<char> unlockedSymbols;
    
    private LanguageMode languageMode = LanguageMode.Secret;
    private List<KeyCode> continueKeys;
    private Image activeSprite;
    private Coroutine chatRoutine;
    private string currentChatInput;
    
    [NaughtyAttributes.Button("Test")]
    private void TestChat()
    { 
        StartChatting("test_chat");
    }

    [NaughtyAttributes.Button("Clear Chat History")]
    private void ClearChatHistory()
    {
        GamePrefs.ClearChatHistory();
    }

    [NaughtyAttributes.Button("Clear Unlocked Symbols")]
    private void ClearUnlockedSymbols()
    {
        GamePrefs.ClearUnlockedSymbols();
    }

    [NaughtyAttributes.Button("Re")]
    private void ReTranslate()
    {
        chatTextBox.text = Translate(currentChatInput, languageMode);
    }

    private void Start()
    {
        textGuesser = GetComponentInChildren<TextGuesser>(true);
        unlockedSymbols = GamePrefs.GetUnlockedSymbols();
        if (unlockedSymbols.Count <= 0)
        {
            unlockedSymbols.AddRange(basicUnlockedSymbols);
            foreach (var symb in basicUnlockedSymbols)
            {
                GamePrefs.SaveToUnlockedSymbol(symb);
            }
        }

        continueKeys = new List<KeyCode>();
        foreach (var code in continueCode.Split(","))
        {
            continueKeys.Add(Enum.Parse<KeyCode>(code));
        }
    }

    private void Update()
    {
        if (!IsOpen)
            return;
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (textGuesser.IsOpen)
            {
                cg.alpha = 1f;
                textGuesser.Close();
            }
            else
            {
                cg.alpha = 0.95f;
                textGuesser.Open(currentChatInput, () =>
                {
                    SaveStringChars(currentChatInput);
                    chatTextBox.text = Translate(currentChatInput, LanguageMode.English);
                });
            }
        }
        
        if (Input.GetKeyDown(KeyCode.T) && !textGuesser.IsOpen)
        {
            languageMode = (LanguageMode)(((int)languageMode + 1) % Enum.GetValues(typeof(LanguageMode)).Length);
            chatTextBox.text = Translate(currentChatInput, languageMode);
        }
        
    }

    private void SaveStringChars(string text)
    {
        List<char> unique = text.Distinct().ToList();
        foreach (var ch in unique)
        {
            GamePrefs.SaveToUnlockedSymbol(ch);
        }
        
        unlockedSymbols = GamePrefs.GetUnlockedSymbols();
    }
    
    public void StartChatting(string chatMapFileName)
    {
        IsOpen = true;
        if(chatRoutine != null)
            StopCoroutine(chatRoutine);
   
        var textAsset = Resources.Load<TextAsset>($"{CHAT_PATH}/{chatMapFileName}");
        if (textAsset == null)
            return;
        ChatData chatData = ChatData.Parse(textAsset.text);
        if (chatData == null)
            return;

        if (GamePrefs.IsNewChat(chatMapFileName))
        {
            Debug.Log("Loading New Chat");
            InitialiseChat(chatData.OnceDialogs, () => GamePrefs.AddToChatHistory(chatMapFileName));
        }
        else
        { 
            Debug.Log("Loading Repeat Chat");
            InitialiseChat(chatData.RepeatDialogs, null);
        }
    }

    public void CloseChatBox()
    {
        if(activeSprite != null)
            activeSprite.gameObject.SetActive(false);
   
        chatBoxArea.SetActive(false);
        IsOpen = false;
        languageMode = LanguageMode.Secret;
    }

    private void InitialiseChat(List<ChatDialog> dialogList, Action onComplete)
    {    
        if(chatRoutine != null)
            StopCoroutine(chatRoutine);

        chatRoutine = StartCoroutine(ChatRoutine(dialogList, onComplete));
    }

    private IEnumerator ChatRoutine(List<ChatDialog> chatDialogs, Action onComplete)
    {
        var indx = 0;
        chatBoxArea.SetActive(true);
        DisplayData(chatDialogs[0]);
        while (true)
        {
            if (IsContinuePressed())
            {
                indx+=1;
                if (indx >= chatDialogs.Count)
                    break;
          
                DisplayData(chatDialogs[indx]);
            }

            yield return null;
        }
   
        onComplete?.Invoke();
        CloseChatBox();
    }

    private bool IsContinuePressed()
    {
        if(textGuesser.IsOpen)
            return false;
        
        foreach (var keyCode in continueKeys)
        {
            if (Input.GetKeyDown(keyCode))
            {
                return true;
            }
        }

        return false;
    }

    private void DisplayData(ChatDialog data)
    {
        nameTextBox.text = data.Name;
        chatTextBox.text = Translate(data.Text, languageMode);
        chatTextBox.ForceMeshUpdate(false,true);
        LoadCharacterImage(data.Img, data.ImgPos);
    }

    private string Translate(string text, LanguageMode languageMode)
    {
        currentChatInput = text;
        if (languageMode == LanguageMode.Secret)
        {
            if (text.StartsWith("`") && text.EndsWith("`"))
                return $"<font={discoveredFontAsset.name}>{text.Replace("`","")}</font>";
        }
   
        string newString = "";
        bool skipping = false;
        foreach (var ch in text)
        {
            if (ch == ' ')
            {
                newString += languageMode == LanguageMode.English?" ": "<nbsp><nbsp>";
                continue;
            }

            if (ch == '`' && !skipping)
            {
                skipping = true;
                continue;
            }
            else if (ch == '`' && skipping)
            {
                skipping = false;
                continue;
            }

            if (skipping)
            {
                newString += ch;
                continue;
            }
            
            string formatted;
            if (this.languageMode == LanguageMode.English)
            {
                if (unlockedSymbols.Contains(char.ToLower(ch)))
                    formatted = $"<font={discoveredFontAsset.name}>{ch}</font>";
                else
                    formatted = HiddenSymbol;
            }
            else
            {
                formatted = $"<font={secretFontAsset.name}>{ch}</font>";
            }

            newString += formatted;
        }
        return newString;
   
    }

    private void LoadCharacterImage(string charImg, ChatDialog.Position position)
    {
        if(activeSprite != null)
            activeSprite.gameObject.SetActive(false);

        activeSprite = availbleSprites[(int)position];
        activeSprite.gameObject.SetActive(true);
        activeSprite.sprite = Resources.Load<Sprite>($"{CHAR_IMG_PATH}/{charImg}");

        if (activeSprite.sprite == null)
            return;
   
        var texRatio = (float)activeSprite.sprite.texture.width / activeSprite.sprite.texture.height;
        var rtf = activeSprite.GetComponent<RectTransform>();
        var sizeDelta = rtf.sizeDelta;
        sizeDelta = new Vector2(sizeDelta.y * texRatio, sizeDelta.y);
        rtf.sizeDelta = sizeDelta;
    }

}