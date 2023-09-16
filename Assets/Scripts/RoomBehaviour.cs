using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class RoomExits
    {
        public Collider col;
        public RoomType targetRoomType;
    }

    [Serializable]
    public class SpawnPointData
    {
        public RoomType fromRoom;
        public Transform spawnPoint;
    }
    
    public class RoomBehaviour : MonoBehaviour
    {
        private RoomTransit _roomTransit;
        public List<SpawnPointData> spawnPointData;
        public Camera roomCamera;
        public RoomType roomTypeName;
        public void Init(RoomTransit rt, RoomType type)
        {
            roomTypeName = type;
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

        public Transform GetSpawnPoint(RoomType rt)
        {
            var xDefault = spawnPointData.FirstOrDefault(x => x.fromRoom == rt);
            if (xDefault != null)
            {
                return xDefault.spawnPoint;
            }
            Debug.LogError($"Did not find for {rt}");
            return spawnPointData[0].spawnPoint;
        }
    }
}