using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class RoomExits
    {
        public Collider col;
        public RoomType targetRoomType;
    }
    public class RoomBehaviour : MonoBehaviour
    {
        private RoomTransit _roomTransit;
        public Transform spawnPoint;
        public Camera roomCamera;
        public void Init(RoomTransit rt)
        {
            _roomTransit = rt;
            if (roomCamera == null)
            {
                roomCamera = GetComponentInChildren<Camera>();
            }
        }

        public RoomBehaviour Activate()
        {
            gameObject.SetActive(true);
            return this;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}