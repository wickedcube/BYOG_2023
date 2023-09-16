using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        public Action PlayerChangedInput;
        
        public float speed = 5;
        public float gravity = -5;

        float velocityY = 0;

        public CharacterController controller;

        private bool isMoving = true;
        public void StopMovingPlayer()
        {
            isMoving = false;
        }
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private Vector3 lastInput = Vector3.zero;
        private void Update()
        {
            if (!isMoving)
            {
                return;
            }
            velocityY += gravity * Time.deltaTime;

            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            if (lastInput != input)
            {
                PlayerChangedInput?.Invoke();
            }

            lastInput = input;
            var camera = RoomTransit.Instance.activeCamera;
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

        public void TeleportPlayer(Vector3 position)
        {
            controller.enabled = false;
            transform.position = position;
            controller.enabled = true;
        }
    }
}