using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TextAsset = UnityEngine.TextAsset;

public class ChatBoxUI : MonoBehaviour
{
   private string CHAT_PATH = "ChatData";
   private string CHAR_IMG_PATH = "Characters";

   [SerializeField] private GameObject chatBoxArea;
   [SerializeField] private TMP_FontAsset secretFontAsset;
   [SerializeField] private TMP_FontAsset discoveredFontAsset;
   [SerializeField] private TMP_Text chatTextBox;
   [SerializeField] private TMP_Text nameTextBox;
   [SerializeField] private string continueCode;

   [SerializeField] private List<Image> availbleSprites;

   [SerializeField] private List<char> basicUnlockedSymbols;
   [SerializeField] private List<char> unlockedSymbols;

   private List<KeyCode> continueKeys;
   private Image activeSprite;
   private Coroutine chatRoutine;
   
   private void Start()
   {
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
       GamePrefs.ClearChatHistory();
   }
   
   public void StartChatting(string chatMapFileName)
   {
      
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
       chatTextBox.text = FormatText(data.Text);
       LoadCharacterImage(data.Img, data.ImgPos);
   }

   private string FormatText(string input)
   {
       string newString = "";
       foreach (var ch in input)
       {
           string formatted;
           if (unlockedSymbols.Contains(char.ToLower(ch)))
               formatted = $"<font={discoveredFontAsset.name}>{ch}</font>";
           else
               formatted = $"<font={secretFontAsset.name}>{ch}</font>";
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
