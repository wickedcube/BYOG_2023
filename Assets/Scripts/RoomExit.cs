using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class RoomExit : MonoBehaviour
    {
        public RoomType exitRoomType;
        private bool isPlayerIn;

        private float lastInputChangedTimestamp;
        private float timeInCollider = 0;

        private IEnumerator Start()
        {
            yield return new WaitUntil(()=>RoomTransit.Instance.activePlayer != null);
            RoomTransit.Instance.activePlayer.PlayerChangedInput += PlayerChangedInput;
        }
        
        private void PlayerChangedInput()
        {
            lastInputChangedTimestamp = Time.time;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                isPlayerIn = true;
                timeInCollider = 0;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!isPlayerIn)
            {
                return;
            }

            timeInCollider += Time.deltaTime;

            if (timeInCollider >= RoomTransit.Instance.waitBeforeChangingRoomsOnPlayerHold )
            {
                if ((Time.time - lastInputChangedTimestamp > 2f))
                {
                    RoomTransit.Instance.TransportPlayerToRoom(exitRoomType);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                isPlayerIn = false;
                timeInCollider = 0;
            }
        }

        private void OnDestroy()
        {
            if (RoomTransit.Instance != null && RoomTransit.Instance.activePlayer != null)
            {
                RoomTransit.Instance.activePlayer.PlayerChangedInput -= PlayerChangedInput;
            }
        }
    }
}