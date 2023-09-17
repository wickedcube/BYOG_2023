using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GamePrefs
{
   private const string KEY_CHAT_HISTORY = ".chatHistory";
   private const string KEY_UNLOCKED_SYMBOLS = ".unlockedSymbols";

   public static List<char> GetUnlockedSymbols()
   {
      List<char> unlockedSymbols = new List<char>();
      if (PlayerPrefs.HasKey(KEY_UNLOCKED_SYMBOLS))
      {
         var list = PlayerPrefs.GetString(KEY_UNLOCKED_SYMBOLS).Split(",").ToList();
         foreach (var itm in list)
         {
            if(itm.Length <= 0)
               continue;
            if(itm == " ")
               continue;
            unlockedSymbols.Add(itm[0]);
         }
      }
      return unlockedSymbols;
   }

   public static void SaveToUnlockedSymbol(char c)
   {
      var list = GetUnlockedSymbols();
      var lower = char.ToLower(c);
      if(!list.Contains(lower))
         list.Add(lower);
      
      PlayerPrefs.SetString(KEY_UNLOCKED_SYMBOLS, string.Join(",", list));
      PlayerPrefs.Save();
   }
   
   public static bool IsNewChat(string chatMapFileName)
   {
      List<string> chatHistory;
      if (PlayerPrefs.HasKey(KEY_CHAT_HISTORY))
         chatHistory = PlayerPrefs.GetString(KEY_CHAT_HISTORY).Split(",").ToList();
      else
         chatHistory = new List<string>();
      
      return !chatHistory.Contains(chatMapFileName);
   }

   public static void AddToChatHistory(string chatMapFileName)
   {
      List<string> chatHistory;
      if (PlayerPrefs.HasKey(KEY_CHAT_HISTORY))
         chatHistory = PlayerPrefs.GetString(KEY_CHAT_HISTORY).Split(",").ToList();
      else
         chatHistory = new List<string>();
      
      chatHistory.Add(chatMapFileName);
      PlayerPrefs.SetString(KEY_CHAT_HISTORY,string.Join(",", chatHistory));
      PlayerPrefs.Save();
   }

   public static void ClearChatHistory()
   {
      PlayerPrefs.DeleteKey(KEY_CHAT_HISTORY);
      PlayerPrefs.Save();
   }
   
   public static void ClearUnlockedSymbols()
   {
      PlayerPrefs.DeleteKey(KEY_UNLOCKED_SYMBOLS);
      PlayerPrefs.Save();
   }
}
