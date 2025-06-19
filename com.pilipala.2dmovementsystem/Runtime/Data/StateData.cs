using UnityEngine;

namespace Pilipala.Movement.Data
{
    [System.Serializable]
    public class StateData
    {
        public float moveSpeedMultiplier = 1f;
        public bool canMove = true;
        public bool canJump = true;
        public bool canShoot = true;
        public bool canAim = true;
        public bool canCrouch = true;
        public bool canClimb = true;
        public float invincibilityDuration = 0f;
        public float stateTimeout = 0f; // 0 means no timeout
    }
}