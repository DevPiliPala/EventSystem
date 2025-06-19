using UnityEngine;
using UnityEngine.InputSystem;
using Pilipala.Movement.Data;
using Pilipala.Movement.Interfaces;
using EventSystem;

namespace Pilipala.Movement.Input
{
    [RequireComponent(typeof(PilipalaPlatformHandler))]
    public class MovementInputHandler : MonoBehaviour, IAimInputProvider, PlayerInputActions.IInGameActions
    {
        [SerializeField] private MovementVariablesSO movementVariables;

        private PlayerInputActions _inputActions;
        private PilipalaPlatformHandler _movementHandler;
        private Vector2 _moveInput;
        private Vector2 _aimInput;
        private bool _isButtonAHeld;
        private bool _isButtonBHeld;
        private bool _isMovingDown;
        private float _buttonAHoldTime;
        private float _buttonBHoldTime;
        private Vector2 _lastAimInput = Vector2.right; // Default to right
        private MovementStateManager _stateManager;
        private Vector2 _currentRawAimInput = Vector2.zero;

        private void Awake()
        {
            _movementHandler = GetComponent<PilipalaPlatformHandler>();
            _inputActions = new PlayerInputActions();
            _inputActions.InGame.SetCallbacks(this);
            _stateManager = GetComponent<MovementStateManager>();

            if (movementVariables == null)
            {
                Debug.LogError("Movement variables not assigned to input handler");
            }

            // Set the movement variables on the movement handler
            if (_movementHandler != null)
            {
                _movementHandler.MovementVariables = movementVariables;
            }
        }

        private void OnEnable()
        {
            _inputActions.InGame.Enable();
        }

        private void OnDisable()
        {
            _inputActions.InGame.Disable();
        }

        private void Update()
        {
            HandleMovementAndAim();
            HandleButtonTimers();

            // Always update aim direction if input is held
            if (_moveInput != Vector2.zero)
            {
                Vector2 snapped = PilipalaPlatformHandler.SnapTo8Directions(_moveInput);
                _movementHandler.SetAimDirection(snapped);
            }

            // Fire MovementAimChanged event if input changes
            if (_moveInput != _currentRawAimInput)
            {
                _currentRawAimInput = _moveInput;
                EventBusHelper.Trigger("Movement.AimChanged", _currentRawAimInput);
            }
        }

        private void HandleMovementAndAim()
        {
            if (movementVariables == null) return;

            // handle movement (only horizontal)
            Vector2 moveDir = new Vector2(_moveInput.x, 0);
            _movementHandler.Move(
                moveDir,
                movementVariables.moveSpeed,
                movementVariables.acceleration,
                movementVariables.deceleration,
                movementVariables.maxSpeed
            );

            // handle aiming (use full 2D input)
            if (_moveInput != Vector2.zero)
            {
                _movementHandler.SetAimDirection(_moveInput);
            }

            // handle crouch/prone when moving down
            bool wasMovingDown = _isMovingDown;
            _isMovingDown = _moveInput.y < -0.5f;
            _movementHandler.SetCrouchState(_isMovingDown);

            // Trigger crouch event if state changed
            if (wasMovingDown != _isMovingDown)
            {
                EventBusHelper.Trigger("Movement.Crouch");
            }
        }

        private void HandleButtonTimers()
        {
            if (movementVariables == null) return;

            if (_isButtonAHeld)
            {
                _buttonAHoldTime += Time.deltaTime;
                _movementHandler.RequestJumpHeld(
                    movementVariables.jumpForce,
                    _buttonAHoldTime / movementVariables.jumpHoldTime,
                    _buttonAHoldTime,
                    movementVariables.jumpHoldTime
                );
            }

            if (_isButtonBHeld)
            {
                _buttonBHoldTime += Time.deltaTime;
            }
        }

        // PlayerInputActions.IInGameActions implementation
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            if (_moveInput != Vector2.zero)
            {
                _lastAimInput = _moveInput;
                Vector2 snapped = PilipalaPlatformHandler.SnapTo8Directions(_moveInput);
                _movementHandler.SetAimDirection(snapped);

                // Trigger movement event
                EventBusHelper.Trigger("Movement.Move");
            }
        }

        public void OnButtonA(InputAction.CallbackContext context)
        {
            if (movementVariables == null) return;

            switch (context.phase)
            {
                case InputActionPhase.Started:
                    _isButtonAHeld = true;
                    _buttonAHoldTime = 0f;
                    _movementHandler.RequestJump(movementVariables.jumpForce);

                    // Trigger jump event
                    EventBusHelper.Trigger("Movement.Jump");
                    break;

                case InputActionPhase.Canceled:
                    _isButtonAHeld = false;
                    _buttonAHoldTime = 0f;
                    break;
            }
        }

        public void OnButtonB(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    _isButtonBHeld = true;
                    _buttonBHoldTime = 0f;
                    EventBusHelper.Trigger("Movement.Attack");
                    break;

                case InputActionPhase.Canceled:
                    _isButtonBHeld = false;
                    _buttonBHoldTime = 0f;
                    break;
            }
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EventBusHelper.Trigger("Movement.WeaponSwitch");
                    break;

                case InputActionPhase.Canceled:
                    break;
            }
        }

        public void OnStart(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EventBusHelper.Trigger("Movement.MenuOpen");
                    break;

                case InputActionPhase.Canceled:
                    break;
            }
        }

        // IAimInputProvider implementation
        public Vector2 CurrentRawAimInput => _currentRawAimInput;

        // Additional properties for external access
        public bool IsShooting() => _isButtonBHeld;
        public bool IsJumping() => _isButtonAHeld;
        public bool IsCrouching() => _isMovingDown;
        public Vector2 GetAimDirection() => _movementHandler.AimDirection;
        public float GetButtonAHoldTime() => _buttonAHoldTime;
        public float GetButtonBHoldTime() => _buttonBHoldTime;
    }
}