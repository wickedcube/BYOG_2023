using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public enum RoomType
    {
        Bedroom,
        Dining,
        Dungeon,
        Study,
        Cellar,
        Drawing,
        None
    }
    
    [Serializable]
    public class RoomToNameMapping
    {
        public RoomType roomTypeName;
        public GameObject roomInScene;
    }
    public class RoomTransit : MonoBehaviour
    {
        public int waitBeforeChangingRoomsOnPlayerHold = 2;
        public static RoomTransit Instance;

        public GameObject playerPrefab;
        public Player activePlayer;
        public Camera activeCamera;
        public List<RoomToNameMapping> roomToNameMappings;

        private RoomBehaviour activeRoom;
        
        public RoomType defaultRoom;
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var roomToNameMapping in roomToNameMappings)
            {
                roomToNameMapping.roomInScene.GetComponent<RoomBehaviour>().Init(this, roomToNameMapping.roomTypeName);
                roomToNameMapping.roomInScene.SetActive(false);

                if (defaultRoom == roomToNameMapping.roomTypeName)
                {
                    activeRoom = roomToNameMapping.roomInScene.GetComponent<RoomBehaviour>().Activate();
                }
            }
            
            SpawnPlayer();
        }

        
        public void SpawnPlayer()
        {
            activePlayer = Instantiate(playerPrefab).GetComponent<Player>();
            TransportPlayerToRoom(defaultRoom);
        }

        public void TransportPlayerToRoom(RoomType xyz)
        {
            var bla = roomToNameMappings.FirstOrDefault(x => x.roomTypeName == xyz);
            if (bla != null)
            {
                RoomType lastRoom = RoomType.None;
                if (activeRoom != null)
                {
                    lastRoom = activeRoom.roomTypeName;
                    activeRoom.GetComponent<RoomBehaviour>().Deactivate();
                }

                var roomB = bla.roomInScene.GetComponent<RoomBehaviour>();
                activeRoom = roomB.Activate();
                activeCamera = activeRoom.roomCamera;
                activePlayer.TeleportPlayer(activeRoom.GetSpawnPoint(lastRoom).position);
            }
            else
            {
                Debug.LogError($"FUCK YOU! GIVE AN ACTUAL ROOM");
            }
        }

        private void OnDisable()
        {
            Instance = null;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}