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
                StateManager.Instance.SetCurrentInteractingChar(gameObject.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                StateManager.Instance.SetCurrentInteractingChar("", true);
            }
        }
    }
}
