using UnityEngine;
using System;
using System.Collections;
using Pilipala.Movement.Data;
using Pilipala.Movement.Interfaces;
using EventSystem;

namespace Pilipala.Movement
{
    [RequireComponent(typeof(PilipalaPlatformHandler))]
    public class MovementStateManager : MonoBehaviour
    {
        [SerializeField] private MovementStateDataSO stateData;

        private MovementState _currentState;
        private StateData _currentStateData;
        private PilipalaPlatformHandler _movementHandler;
        private Coroutine _stateTimeoutCoroutine;
        private Coroutine _invincibilityCoroutine;
        private float _stateTimer;
        private Coroutine _shootingCoroutine;
        public bool isShooting { get; private set; }

        // properties
        public MovementState CurrentState => _currentState;
        public StateData CurrentStateData => _currentStateData;
        public bool IsInvincible => _currentState == MovementState.Invincible;
        public float StateTimer => _stateTimer;

        private void Awake()
        {
            _movementHandler = GetComponent<PilipalaPlatformHandler>();

            if (stateData == null)
            {
                Debug.LogError("Movement state data not assigned");
            }
            SetState(MovementState.Normal);
        }

        private void Update()
        {
            if (_stateTimer > 0f)
            {
                _stateTimer -= Time.deltaTime;
            }
        }

        public void SetState(MovementState state)
        {
            if (_currentState == state) return;

            _currentState = state;
            _currentStateData = GetStateData(state);

            if (_currentStateData == null)
            {
                Debug.LogError($"No state data found for state: {state}");
                return;
            }

            // Handle state-specific logic
            switch (state)
            {
                case MovementState.Normal:
                    // reset any state-specific flags
                    EventBusHelper.Trigger("Movement.StateNormal");
                    break;

                case MovementState.Shooting:
                    // start shooting state timeout
                    if (_currentStateData.stateTimeout > 0f)
                    {
                        _stateTimeoutCoroutine = StartCoroutine(StateTimeoutRoutine());
                    }
                    EventBusHelper.Trigger("Movement.StateShooting");
                    break;

                case MovementState.Prone:
                    _movementHandler.SetCrouchState(true);
                    EventBusHelper.Trigger("Movement.StateProne");
                    break;

                case MovementState.Jumping:
                    // handle jumping state setup
                    EventBusHelper.Trigger("Movement.StateJumping");
                    break;

                case MovementState.Dead:
                    EventBusHelper.Trigger("Movement.StateDead");
                    break;

                case MovementState.Invincible:
                    EventBusHelper.Trigger("Movement.InvincibilityStarted");
                    _invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());
                    break;
            }

            // Trigger state changed event
            EventBusHelper.Trigger("Movement.StateChanged", state);
        }

        private StateData GetStateData(MovementState state)
        {
            if (stateData == null) return null;

            return state switch
            {
                MovementState.Normal => stateData.normalState,
                MovementState.Shooting => stateData.shootingState,
                MovementState.Prone => stateData.proneState,
                MovementState.Jumping => stateData.jumpingState,
                MovementState.Invincible => stateData.invincibleState,
                MovementState.Dead => stateData.deadState,
                _ => stateData.normalState
            };
        }

        private IEnumerator StateTimeoutRoutine()
        {
            yield return new WaitForSeconds(_currentStateData.stateTimeout);
            SetState(MovementState.Normal);
            _stateTimeoutCoroutine = null;
        }

        private IEnumerator InvincibilityRoutine()
        {
            yield return new WaitForSeconds(_currentStateData.invincibilityDuration);
            SetState(MovementState.Normal);
            _invincibilityCoroutine = null;
        }

        public bool CanPerformAction(MovementAction action)
        {
            if (_currentStateData == null)
            {
                Debug.LogWarning($"MovementStateManager: _currentStateData is null when checking action {action}. Returning false.");
                return false;
            }
            return action switch
            {
                MovementAction.Move => _currentStateData.canMove,
                MovementAction.Jump => _currentStateData.canJump,
                MovementAction.Shoot => _currentStateData.canShoot,
                MovementAction.Aim => _currentStateData.canAim,
                MovementAction.Crouch => _currentStateData.canCrouch,
                MovementAction.Climb => _currentStateData.canClimb,
                _ => false
            };
        }

        public float GetMoveSpeedMultiplier()
        {
            return _currentStateData.moveSpeedMultiplier;
        }

        public void TriggerShooting(float duration)
        {
            if (_shootingCoroutine != null)
            {
                StopCoroutine(_shootingCoroutine);
            }
            _shootingCoroutine = StartCoroutine(ShootingRoutine(duration));
        }

        private IEnumerator ShootingRoutine(float duration)
        {
            isShooting = true;
            EventBusHelper.Trigger("Movement.ShootingChanged", true);
            yield return new WaitForSeconds(duration);
            isShooting = false;
            EventBusHelper.Trigger("Movement.ShootingChanged", false);
            _shootingCoroutine = null;
        }
    }

    // enum for movement actions
    public enum MovementAction
    {
        Move,
        Jump,
        Shoot,
        Aim,
        Crouch,
        Climb
    }
}