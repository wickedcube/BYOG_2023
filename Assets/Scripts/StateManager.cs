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
        { Guest.Uma, 1 },
        { Guest.Naina, 1 },
        { Guest.Anil, 1 },
        { Guest.Ishaan, 1 },
    };
    
    private Guest? _lastInteractedGuestN;
    private HashSet<Guest> _translatedPhase1Guests = new();
    private HashSet<Guest> _translatedPhase2Guests = new();
    public  List<Transform> phase1Guests;
    public List<Transform> phase2Guests;
    public List<Transform> phase3Guests;
    public Transform solver;

    public static StateManager Instance;

    private void Start()
    {
        Instance = this;
        ChatBoxUI.Instance.OnChatBoxClosed += () =>
        {
            var lastInteractedGuest = _lastInteractedGuestN ?? Guest.Arvind;
            if (lastInteractedGuest == Guest.Arvind && 
                CharNameToStateDict[lastInteractedGuest] == 1 &&
                !_translatedPhase1Guests.Contains(Guest.Arvind))
            {
                ChatBoxUI.Instance.UnlockAtStart("defrost");
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

            if (_translatedPhase1Guests.Count == Enum.GetNames(typeof(Guest)).Length - 2)
            {
                MoveCharToNextState(Guest.Priya);
                MoveCharToNextState(Guest.Raghav);
                MoveCharToNextState(Guest.Uma);
                MoveCharToNextState(Guest.Naina);
                MoveCharToNextState(Guest.Anil);
                MoveCharToNextState(Guest.Ishaan);
            }

            if (_translatedPhase2Guests.Count == Enum.GetNames(typeof(Guest)).Length - 2)
            {
                solver.gameObject.SetActive(true);
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
