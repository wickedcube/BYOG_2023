using System;
using System.Collections.Generic;
using UnityEngine;

public enum Guest
{
    Arvind,
    Priya,
    Raghav,
    Uma,
    Naina,
    Anil,
    Ishaan,
}

public class StateManager : MonoBehaviour
{
    Dictionary<Guest, int> CharNameToStateDict = new ()
    {
        { Guest.Arvind, 1 },
        { Guest.Priya, 1 },
        { Guest.Raghav, 1 },
        { Guest.Uma, 2 },
        { Guest.Naina, 1 },
        { Guest.Anil, 1 },
        { Guest.Ishaan, 1 },
    };
    
    private Guest? _lastInteractedGuestN;
    private HashSet<Guest> _translatedPhase1Guests = new();
    private HashSet<Guest> _translatedPhase2Guests = new();
    public Transform phase1Guests;
    public Transform phase2Guests;
    public Transform solver;

    public static StateManager Instance;
    public Transform book;

    private void Start()
    {
        Instance = this;

        string introString = "Arvind Kapoor (Found dead in the study): Self-made entrepreneur in tech and arts. Known for generosity and charisma but not without controversies. Quote: \"Life is a game of chess, and I play for the endgame.\"\n\nNaina Sahni: Travel blogger, childhood friend of Arvind. Families connected through business. Vivacious and lively.\n\nPriya Desai: Author, college friend of Arvind. Calm and elegant. Rumored past romance with Arvind.\n\nAnil Bhatia: Artist, art school roommate of Arvind. Dreamer with a sketchbook.\n\nRaghav Mehta: Venture capitalist, recent business disagreement with Arvind. Old school friend from cricket days. Sharp-witted.\n\nDr. Uma Krishnan: College professor, taught Arvind and Priya. Stern but fair.\n\nIshaan Verma: Theater director, collaborated with Arvind. Mix of respect and rivalry.";
        ChatBoxUI.Instance.AddToJournal(introString);
        
        ChatBoxUI.Instance.OnChatBoxClosed += () =>
        {
            var lastInteractedGuest = _lastInteractedGuestN ?? Guest.Arvind;
            if (lastInteractedGuest == Guest.Arvind && 
                CharNameToStateDict[lastInteractedGuest] == 1 &&
                !_translatedPhase1Guests.Contains(Guest.Arvind))
            {
                ChatBoxUI.Instance.UnlockAtStart("dream");
                book.gameObject.SetActive(false);
            }
        };

        ChatBoxUI.Instance.OnSuccessfulTranslation += () =>
        {
            var lastInteractedGuest = _lastInteractedGuestN ?? Guest.Arvind;
            if (CharNameToStateDict[lastInteractedGuest] == 1)
            {
                _translatedPhase1Guests.Add(lastInteractedGuest);    
            }
            else if (CharNameToStateDict[lastInteractedGuest] == 2)
            {
                _translatedPhase2Guests.Add(lastInteractedGuest);
            }

            if (CharNameToStateDict[lastInteractedGuest] == 1 && _translatedPhase1Guests.Count == Enum.GetNames(typeof(Guest)).Length - 2)
            {
                MoveToPhase2();   
            }
            
            if (CharNameToStateDict[lastInteractedGuest] == 1 && CharNameToStateDict[lastInteractedGuest] == 1 && ChatBoxUI.Instance.UnlockedSymbolsCount == 28)
            {
                MoveToPhase2();
            }

            if (_translatedPhase2Guests.Count == Enum.GetNames(typeof(Guest)).Length - 2)
            {
                phase2Guests.gameObject.SetActive(false);
                solver.gameObject.SetActive(true);
            }
        };
    }

    private void MoveToPhase2()
    {
        phase1Guests.gameObject.SetActive(false);
                
        MoveCharToNextState(Guest.Priya);
        MoveCharToNextState(Guest.Raghav);
        MoveCharToNextState(Guest.Uma);
        MoveCharToNextState(Guest.Naina);
        MoveCharToNextState(Guest.Anil);
        MoveCharToNextState(Guest.Ishaan);
                
        phase2Guests.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _lastInteractedGuestN != null && !ChatBoxUI.Instance.IsOpen)
        {
            ChatBoxUI.Instance.StartChatting(GetChatAssetName());
        }
    }

    public void SetCurrentInteractingChar(string gameObjectName, bool setNull = false)
    {
        Debug.Log("Setting current player to " + gameObjectName == "" ? "Nothing" : gameObjectName);
        if (setNull)
        {
            _lastInteractedGuestN = null;
            return;
        }

        Enum.TryParse(gameObjectName, out Guest parsedGuest);
        _lastInteractedGuestN = parsedGuest;
    }
    
    public void ResetCurrentInteractingChar(string gameObjectName)
    {
        _lastInteractedGuestN = null;
    }

    private string GetChatAssetName()
    {
        if (_lastInteractedGuestN != null)
        {
            var p = (int)_lastInteractedGuestN;
            var s = CharNameToStateDict[_lastInteractedGuestN ?? Guest.Arvind];
            return "Char_"+p+"_"+s;
        }
        else
        {
            Debug.LogError("Last interacted char not set");
            return "";
        }
    }

    private void MoveCharToNextState(Guest guest)
    {
        _lastInteractedGuestN = guest;
        if (CharNameToStateDict.ContainsKey(guest))
        {
            CharNameToStateDict[guest]++;
        }
        else
        {
            CharNameToStateDict.Add(guest, 0);
        }
    }
}
