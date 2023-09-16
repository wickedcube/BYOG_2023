using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public class ChatDialog
{
    [Serializable]
    public enum Position
    {
        Left,
        Centre,
        Right
    }
    
    public string Name;
    public string Img;
    public string Text;
    public Position ImgPos;

}

public class ChatData
{
    private const string SYMB_ONCE = "[once]";
    private const string SYMB_REPEEAT = "[repeat]";

    private const string SPEECH_COMP_SEPERATOR = " : ";
    private const string IMG_COMP_SEPERATOR = "@";
    private const string IMG_BOX_OPEN = "[";
    private const string IMG_BOX_CLOSE = "]";
    
    
    public List<ChatDialog> OnceDialogs;
    public List<ChatDialog> RepeatDialogs;

    public static ChatData Parse(string chatDataText)
    { 
        var onceStartIndx = chatDataText.IndexOf(SYMB_ONCE);
        var repeatStartIndx = chatDataText.IndexOf(SYMB_REPEEAT);
        var repeatEndIndx = repeatStartIndx + SYMB_REPEEAT.Length + 1;
        var onceData = chatDataText.Substring(onceStartIndx + SYMB_ONCE.Length + 1, repeatStartIndx - SYMB_REPEEAT.Length).Trim();
        var repeatData = chatDataText.Substring(repeatEndIndx, chatDataText.Length - repeatEndIndx).Trim();
        
        return new ChatData()
        {
            OnceDialogs = ParseDialogData(onceData),
            RepeatDialogs = ParseDialogData(repeatData)
        };
    }
    
    private static List<ChatDialog> ParseDialogData(string data)
    {
        string[] lines = data.Split("\n");
        List<ChatDialog> dialogTexts = new List<ChatDialog>();
        foreach (var line in lines)
        {
            if(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                continue;
            
            var dialogData = new ChatDialog();
            string[] stringComp = line.Split(SPEECH_COMP_SEPERATOR);
            dialogData.Text = stringComp[1].Trim();

            var imgStart = stringComp[0].IndexOf(IMG_BOX_OPEN, StringComparison.Ordinal);
            var imgLength = stringComp[0].IndexOf(IMG_BOX_CLOSE, StringComparison.Ordinal) - imgStart - 1;

            var imgComp = stringComp[0].Substring(imgStart + 1, imgLength).Trim();
            if (!(string.IsNullOrWhiteSpace(imgComp) || string.IsNullOrEmpty(imgComp)))
            {
                if (!imgComp.Contains("@"))
                {
                    dialogData.Img = imgComp;
                }
                else
                {
                    string[] imgData = imgComp.Split(IMG_COMP_SEPERATOR);
                    dialogData.Img = imgData[0].Trim();
                    string imgPosData = imgData[1].Trim();
                    dialogData.ImgPos =
                        Enum.Parse<ChatDialog.Position>(
                            $"{char.ToUpper(imgPosData[0])}{imgPosData.Substring(1, imgPosData.Length - 1).ToLower()}");
                }
            }

            dialogData.Name = stringComp[0].Substring(0, imgStart - 1).Trim();
            dialogTexts.Add(dialogData);
        }

        return dialogTexts;
    }
}
