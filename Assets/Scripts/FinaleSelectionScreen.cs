using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class FinaleSelectionScreen : MonoBehaviour
    {
        public Transform buttonParent;
        public Transform correctButton;
        public string correctAnswer;
        public Transform chosenButton;
        public GameObject inputFieldPrefab;

        private void Start()
        {
            foreach (Transform o in buttonParent)
            {
                o.GetComponent<Button>().onClick.AddListener(() =>
                {
                    MarkChosenButtonSelected(false);
                    chosenButton = o;
                    if (instInputField != null)
                    {
                        Destroy(instInputField);
                    }
                    MarkChosenButtonSelected(true);
                    instInputField = Instantiate(inputFieldPrefab, transform);
                    instInputField.GetComponentInChildren<TMP_InputField>().onValueChanged.AddListener(Call);
                });
            }
        }


        private GameObject instInputField;

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Return) && instInputField != null)
            {
                if (chosenButton == correctButton && instInputField.GetComponentInChildren<TMP_InputField>().text == correctAnswer)
                {
                    Debug.LogError($"game won!");
                }
                else
                {
                    Debug.LogError($"Game lost!");
                    CleanInputField();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) )
            {
                if (instInputField != null)
                {
                    CleanInputField();
                }
                else
                {
                    CleanSelf();
                }
            }
        }

        private void CleanSelf()
        {
            Destroy(gameObject);
        }

        private void CleanInputField()
        {
            Destroy(instInputField);
            instInputField = null;
            MarkChosenButtonSelected(false);
        }

        public void MarkChosenButtonSelected(bool selected)
        {
            if (selected && chosenButton)
            {
                chosenButton.GetComponent<Image>().color = Color.black;
            }
            else if(chosenButton)
            {
                chosenButton.GetComponent<Image>().color = Color.white;
            }
        }
        private void Call(string arg0)
        {
        }
    }
}