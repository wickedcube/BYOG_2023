using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        public float speed = 5;
        public float gravity = -5;

        float velocityY = 0;

        CharacterController controller;

        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            velocityY += gravity * Time.deltaTime;

            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            var camera = Camera.main;
            var camForward = camera.transform.forward;
            var forward = Vector3.ProjectOnPlane(camForward, Vector3.up);


            var horizontAL = Quaternion.AngleAxis(90, Vector3.up) * forward;


            var vel = Vector3.zero;


            if (input.x != 0)
            {
                vel = horizontAL * (speed * (input.x > 0 ? 1 : -1));
            }


            if (input.z != 0)
            {
                vel += forward * (speed * (input.z > 0 ? 1 : -1));
            }

            vel += Vector3.down * gravity;
            controller.Move(vel * Time.deltaTime);
        }
    }
}