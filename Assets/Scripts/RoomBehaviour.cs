using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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
        public const float minTimeToMove = 0.3f;
        public const float maxTimeToMove = 0.8f;
        
        private RoomTransit _roomTransit;
        public List<SpawnPointData> spawnPointData;
        public Camera roomCamera;
        public RoomType roomTypeName;

        private List<TweenerCore<Vector3, Vector3, VectorOptions>> tweens = new();
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
            StartBakchodi();
            return this;
        }

        public void Deactivate()
        {
            if (tweens != null)
            {
                foreach (var tweenerCore in tweens)
                {
                    if (tweenerCore != null)
                    {
                        tweenerCore.Kill();
                    }
                }
                tweens = new();
            }
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


        private Dictionary<Transform, Vector3> originalMapping;
        public void StartBakchodi()
        {
            if (originalMapping == null)
            {
                GenerateMapping();
            }
            DoBakchodi();
        }
        

        private void DoBakchodi()
        {
            
            var basePos = transform.GetChild(0).position;
            var list = originalMapping.OrderBy(x => (x.Value - basePos).sqrMagnitude);

            float minDist = Mathf.Infinity;
            float maxDist = 0;
            foreach (var (key, value) in list)
            {
                var newY = Mathf.Abs(value.y - basePos.y);
                key.transform.position = new Vector3(value.x, (basePos.y - newY), value.z);

                var dist = Vector3.Distance(key.transform.position, value);
                if (dist > maxDist)
                {
                    maxDist = dist;
                }
                else if(dist < minDist)
                {
                    minDist = dist;
                }
            }
            
            foreach (var (key, value) in list)
            {
                if (key.TryGetComponent(out Collider col))
                {
                    col.enabled = false;
                }
                var dist = Vector3.Distance(key.transform.position, value);
                var remapTime = map(dist, minDist, maxDist,maxTimeToMove, minTimeToMove);
                TweenerCore<Vector3, Vector3, VectorOptions> x = key.DOMove(value, remapTime).SetEase(Ease.InOutExpo).OnComplete(() =>
                {
                    key.GetComponent<Collider>().enabled = true;
                });
                tweens.Add(x);
            }
        }

        private void GenerateMapping()
        {
            var parent = transform.Find("Default");
            originalMapping = new();
            foreach (Transform o in parent)
            {
                originalMapping.TryAdd(o, o.transform.position);
            }
        }
        
        float map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s-a1)*(b2-b1)/(a2-a1);
        }
    }
}