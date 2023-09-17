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
    Ishaan
}

public class StateManager : MonoBehaviour
{
    Dictionary<Guest, int> CharNameToStateDict = new ()
    {
        { Guest.Arvind, 1 },
        { Guest.Priya, 1 },
        { Guest.Raghav, 1 },
        { Guest.Uma, 1 },
        { Guest.Naina, 1 },
        { Guest.Anil, 1 },
        { Guest.Ishaan, 1 },
    };

    Dictionary<(Guest, int), Guest> TriggerAnotherCharacterNext = new ()
    {
        { (Guest.Naina, 2), Guest.Raghav },
        { (Guest.Priya, 2), Guest.Uma },
        { (Guest.Uma, 2), Guest.Anil },
    };
    
    private Guest? _lastInteractedGuestN;
    private HashSet<Guest> _translatedPhase1Guests = new();
    private HashSet<Guest> _translatedPhase2Guests = new();
    public  List<Transform> phase1PosList;
    private List<Vector3> _phase2Pos;
    private List<Vector3> _phase3Pos;

    public static StateManager Instance;

    private void Start()
    {
        Instance = this;
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

            if (TriggerAnotherCharacterNext.ContainsKey((lastInteractedGuest, CharNameToStateDict[lastInteractedGuest])))
            {
                MoveCharToNextState(TriggerAnotherCharacterNext[(lastInteractedGuest, CharNameToStateDict[lastInteractedGuest])]);    
            }

            if (_translatedPhase1Guests.Count == Enum.GetNames(typeof(Guest)).Length - 2)
            {
                _translatedPhase1Guests.Clear();
                MoveCharToNextState(Guest.Naina);
                MoveCharToNextState(Guest.Priya);
                MoveCharToNextState(Guest.Ishaan);
            }

            if (_translatedPhase2Guests.Count == Enum.GetNames(typeof(Guest)).Length - 1)
            {
                MoveCharToNextState(Guest.Priya);
                MoveCharToNextState(Guest.Raghav);
                MoveCharToNextState(Guest.Uma);
                MoveCharToNextState(Guest.Naina);
                MoveCharToNextState(Guest.Anil);
                MoveCharToNextState(Guest.Ishaan);
            }
        };
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
        if (parsedGuest == Guest.Arvind)
        {
            Debug.LogError("Arvind is a dead man");
        }
        
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
