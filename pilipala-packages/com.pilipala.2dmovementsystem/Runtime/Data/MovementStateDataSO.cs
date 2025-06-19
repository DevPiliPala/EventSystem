using UnityEngine;

namespace Pilipala.Movement.Data
{
    [CreateAssetMenu(fileName = "MovementStateData", menuName = "ScriptableObjects/Movement/MovementStateData", order = 2)]
    public class MovementStateDataSO : ScriptableObject
    {
        [Header("State Settings")]
        public StateData normalState = new StateData();
        public StateData shootingState = new StateData { moveSpeedMultiplier = 0.7f };
        public StateData proneState = new StateData { moveSpeedMultiplier = 0.5f, canJump = false };
        public StateData jumpingState = new StateData { canCrouch = false, canClimb = false };
        public StateData climbingState = new StateData { canJump = false, canCrouch = false };
        public StateData invincibleState = new StateData { invincibilityDuration = 2f };
        public StateData deadState = new StateData
        {
            canMove = false,
            canJump = false,
            canShoot = false,
            canAim = false,
            canCrouch = false,
            canClimb = false
        };
    }
}