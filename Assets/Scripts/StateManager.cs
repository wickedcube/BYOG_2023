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
    public Dictionary<Guest, int> CharNameToStateDict;

    Dictionary<(Guest, int), Guest> TriggerAnotherCharacterNext = new ()
    {
        { (Guest.Naina, 2), Guest.Raghav },
        { (Guest.Priya, 2), Guest.Uma },
        { (Guest.Uma, 2), Guest.Anil },
    };
    
    private Guest _lastInteractedGuest;
    private HashSet<Guest> _translatedPhase1Guests;
    private HashSet<Guest> _translatedPhase2Guests;
    private List<Vector3> _phase1Pos;
    private List<Vector3> _phase2Pos;
    private List<Vector3> _phase3Pos;

    private void Start()
    {
        ChatBoxUI.Instance.OnSuccessfulTranslation += () =>
        {
            if (CharNameToStateDict[_lastInteractedGuest] == 1)
            {
                _translatedPhase1Guests.Add(_lastInteractedGuest);    
            }
            else if (CharNameToStateDict[_lastInteractedGuest] == 2)
            {
                _translatedPhase2Guests.Add(_lastInteractedGuest);
            }

            if (TriggerAnotherCharacterNext.ContainsKey((_lastInteractedGuest, CharNameToStateDict[_lastInteractedGuest])))
            {
                MoveCharToNextState(TriggerAnotherCharacterNext[(_lastInteractedGuest, CharNameToStateDict[_lastInteractedGuest])]);    
            }

            if (_translatedPhase1Guests.Count == Enum.GetNames(typeof(Guest)).Length - 1)
            {
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

    public string GetChatAssetName(string gameObjectName)
    {
        Enum.TryParse(gameObjectName, out Guest parsedGuest);
        if (parsedGuest == Guest.Arvind)
        {
            Debug.LogError("Arvind is a dead man");
        }

        var p = (int)parsedGuest;
        var s = CharNameToStateDict[parsedGuest];
        return "Char_"+p+"_"+s;
    }

    private void MoveCharToNextState(Guest guest)
    {
        _lastInteractedGuest = guest;
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
