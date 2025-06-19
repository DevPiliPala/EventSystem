using UnityEngine;

namespace Pilipala.Movement.Data
{
    [CreateAssetMenu(fileName = "MovementVariables", menuName = "ScriptableObjects/Movement/MovementVariables", order = 1)]
    public class MovementVariablesSO : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Base movement speed")]
        public float moveSpeed = 8f;

        [Tooltip("Movement speed while crouching")]
        [Range(1f, 8f)]
        public float crouchMoveSpeed = 4f;

        [Tooltip("How quickly the entity reaches max speed (higher = snappier starts)")]
        [Range(50f, 200f)]
        public float acceleration = 100f;

        [Tooltip("How quickly the entity slows down (lower = more slide)")]
        [Range(10f, 80f)]
        public float deceleration = 40f;

        [Tooltip("Maximum movement speed the entity can reach")]
        [Range(5f, 15f)]
        public float maxSpeed = 8f;

        [Header("Aiming")]
        [Tooltip("How quickly the aim direction transitions (higher = faster)")]
        [Range(5f, 30f)]
        public float aimTransitionSpeed = 15f;

        [Tooltip("Available aim angles in degrees (0 = right, 90 = up, etc)")]
        public float[] aimAngles = new float[] { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };

        [Header("Jump")]
        [Tooltip("Initial force applied when jumping")]
        [Range(5f, 20f)]
        public float jumpForce = 10f;

        [Tooltip("How long the jump button can be held for variable jump height")]
        [Range(0.1f, 1f)]
        public float jumpHoldTime = 0.3f;
    }
}