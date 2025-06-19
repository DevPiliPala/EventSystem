using UnityEngine;

namespace Pilipala.Movement.Interfaces
{
    public interface IMoveHandler
    {
        #region Core Movement
        public void Move(Vector2 direction, float speed, float acceleration, float deceleration, float maxSpeed);
        public void Jump(float jumpForce);
        #endregion

        #region State Queries
        public Vector2 GetPosition();
        public Vector2 GetVelocity();
        public Quaternion GetRotation();
        #endregion

        #region Animation State
        public bool IsGrounded { get; }
        public bool IsJumping { get; }
        public bool IsMoving { get; }
        public Vector2 MoveDirection { get; }
        public float CurrentSpeed { get; }
        public float JumpHoldTime { get; }
        #endregion

        #region Flip State
        public bool IsFlipping { get; }
        public bool CanFlip { get; }
        #endregion
    }
}