using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Billboard : MonoBehaviour
    {
        private void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}