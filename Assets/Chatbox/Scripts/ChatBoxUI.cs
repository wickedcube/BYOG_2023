using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
    [FormerlySerializedAs("helpButtons")] [SerializeField] private GameObject chatHelpButtons;
    [SerializeField] private GameObject journalGameObject;
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private TMP_Text historyLabel;
    [SerializeField] private Animator chatBoxAnim;
    [SerializeField] private GameObject chatBoxArea;
    [SerializeField] private TMP_FontAsset secretFontAsset;
    [SerializeField] private TMP_FontAsset discoveredFontAsset;
    [SerializeField] private TMP_Text chatTextBox;
    [SerializeField] private TMP_Text chatOgOnTranslateBox;
    [SerializeField] private TMP_Text nameTextBox;
    [SerializeField] private TextGuesser textGuesser;
    [SerializeField] private string continueCode;
    [SerializeField] private string HiddenSymbol = "[]";
    [SerializeField] private List<Image> availbleSprites;
    [SerializeField] private List<char> basicUnlockedSymbols;
    [SerializeField] private List<char> unlockedSymbols;
    [SerializeField] private List<string> readFilesNames;
    
    private LanguageMode languageMode = LanguageMode.Secret;
    private List<KeyCode> continueKeys;
    private Image activeSprite;
    private Coroutine chatRoutine;

    private string currentSpeaker;
    private string currentChatInput;

    public Action OnSuccessfulTranslation;
    public Action OnChatBoxClosed;
    
    
    [NaughtyAttributes.Button("Test")]
    private void TestChat()
    { 
        StartChatting("test_chat");
    }

    // [NaughtyAttributes.Button("Clear Chat History")]
    // private void ClearChatHistory()
    // {
    //     GamePrefs.ClearChatHistory();
    // }
    //
    // [NaughtyAttributes.Button("Clear Unlocked Symbols")]
    // private void ClearUnlockedSymbols()
    // {
    //     GamePrefs.ClearUnlockedSymbols();
    // }

    [NaughtyAttributes.Button("Re")]
    private void ReTranslate()
    {
        chatTextBox.text = Translate(currentChatInput, languageMode);
    }

    private void Start()
    {
        AudioManager.Instance.PlayBG(AudioManager.ClipTypes.GamePlayBG_3);
        textGuesser = GetComponentInChildren<TextGuesser>(true);
        textGuesser.OnClosed += () =>
        {
            cg.alpha = 1f;
        };
        // unlockedSymbols = GamePrefs.GetUnlockedSymbols();
        if (unlockedSymbols.Count <= 0)
        {
            unlockedSymbols.AddRange(basicUnlockedSymbols);
        }

        continueKeys = new List<KeyCode>();
        foreach (var code in continueCode.Split(","))
        {
            continueKeys.Add(Enum.Parse<KeyCode>(code));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !textGuesser.IsOpen && !IsOpen)
        {
            AudioManager.Instance.PlaySFX(AudioManager.ClipTypes.Click);
            journalGameObject.SetActive(!journalGameObject.activeSelf);
        }
        
        if (!IsOpen)
            return;
        
        if (Input.GetKeyDown(KeyCode.G) && !textGuesser.IsOpen)
        {
            AudioManager.Instance.PlaySFX(AudioManager.ClipTypes.Click);
            cg.alpha = 0.95f;
            journalGameObject.SetActive(false);
            textGuesser.Open(currentChatInput, () =>
            {
                SaveStringChars(currentChatInput);
                languageMode = LanguageMode.English;
                chatTextBox.text = Translate(currentChatInput, languageMode);
                historyLabel.text += $"{currentSpeaker} : {chatTextBox.text}\n";
                historyLabel.text += $"{currentSpeaker} : {Translate(currentChatInput, LanguageMode.Secret)}\n\n";
                OnSuccessfulTranslation?.Invoke();
                textGuesser.Close();
                AudioManager.Instance.PlaySFX(AudioManager.ClipTypes.Translated);
            });
        }

        if (Input.GetKeyDown(KeyCode.T) && !textGuesser.IsOpen)
        {
            AudioManager.Instance.PlaySFX(AudioManager.ClipTypes.Click);
            languageMode = (LanguageMode)(((int)languageMode + 1) % Enum.GetValues(typeof(LanguageMode)).Length);
            chatTextBox.text = Translate(currentChatInput, languageMode);
            chatOgOnTranslateBox.gameObject.SetActive(languageMode == LanguageMode.English);
            if (languageMode == LanguageMode.English)
            {
                chatOgOnTranslateBox.text = Translate(currentChatInput, LanguageMode.Secret);
            }
        }
        
    }

    private void SaveStringChars(string text)
    {
        List<char> unique = text.Distinct().ToList();
        foreach (var ch in unique)
        {
            var cLower = char.ToLower(ch);
            unlockedSymbols.Add(cLower);
        }
    }
    
    public void StartChatting(string chatMapFileName)
    {
        IsOpen = true;
        chatHelpButtons.gameObject.SetActive(true);
        if(chatRoutine != null)
            StopCoroutine(chatRoutine);
        
        var textAsset = Resources.Load<TextAsset>($"{CHAT_PATH}/{chatMapFileName}");
        if (textAsset == null)
            return;
        ChatData chatData = ChatData.Parse(textAsset.text);
        if (chatData == null)
            return;

        if (!readFilesNames.Contains(chatMapFileName))
        {
            Debug.Log("Loading New Chat");
            InitialiseChat(chatData.OnceDialogs, () => readFilesNames.Add(chatMapFileName));
        }
        else
        { 
            Debug.Log("Loading Repeat Chat");
            InitialiseChat(chatData.RepeatDialogs, null);
        }
        chatBoxAnim.SetBool("Open", IsOpen);
    }

    public void CloseChatBox()
    {
        if(activeSprite != null)
            activeSprite.gameObject.SetActive(false);

        IsOpen = false;
        languageMode = LanguageMode.Secret;
        if(textGuesser.IsOpen)
            textGuesser.Close();
        OnChatBoxClosed?.Invoke();
        chatBoxAnim.SetBool("Open", IsOpen);
        StartCoroutine(DelayedCall(() =>
        {
            chatBoxArea.SetActive(false);
        }, 0.2f));
        chatHelpButtons.gameObject.SetActive(false);
        chatOgOnTranslateBox.gameObject.SetActive(false);
    }

    IEnumerator DelayedCall(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
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
            if (textGuesser.IsOpen)
            {
                yield return new WaitForSeconds(1);
            }
            
            if (IsContinuePressed())
            {
                indx+=1;
                if (indx >= chatDialogs.Count)
                    break;
          
                DisplayData(chatDialogs[indx]);
                AudioManager.Instance.PlaySFX(AudioManager.ClipTypes.ChatBoxNext);
            }

            yield return null;
        }
   
        onComplete?.Invoke();
        CloseChatBox();
    }

    private bool IsContinuePressed()
    {
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
        currentSpeaker = data.Name;
        
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
                newString += languageMode == LanguageMode.English?" ": "<nbsp>";
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
            if (languageMode == LanguageMode.English)
            {
                var lowerch = char.ToLower(ch);
                if (unlockedSymbols.Contains(lowerch) || basicUnlockedSymbols.Contains(lowerch))
                    formatted = $"<font={discoveredFontAsset.name}>{ch}</font>";
                else
                    formatted = HiddenSymbol;
            }
            else
            {
                formatted = $"<mspace=15px> <font={secretFontAsset.name}>{ch}</font>";
            }

            newString += formatted;
        }
        return newString;
   
    }

    public void UnlockAtStart(string s)
    {
        foreach (var c in s)
        {
            var cLower = char.ToLower(c);
            if (!unlockedSymbols.Contains(cLower))
            {
                unlockedSymbols.Add(cLower);
            }
        }
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