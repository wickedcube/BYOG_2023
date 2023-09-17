using System;
using DefaultNamespace;
using UnityEngine;

namespace DefaultNamespace
{
    public class ChatTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                other.gameObject.GetComponent<Player>().interactETransform.gameObject.SetActive(true);
                StateManager.Instance.SetCurrentInteractingChar(gameObject.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                other.gameObject.GetComponent<Player>().interactETransform.gameObject.SetActive(false);
                StateManager.Instance.SetCurrentInteractingChar("", true);
            }
        }
    }
}
