using UnityEngine;
using Pilipala.Movement.Interfaces;
using Pilipala.Movement.Data;
using EventSystem;

namespace Pilipala.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PilipalaPlatformHandler : MonoBehaviour, IMoveHandler
    {
        [SerializeField] private float groundCheckRadius = 0.1f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask platformLayer;
        [SerializeField] private float standingColliderHeight = 1f;
        [SerializeField] private float crouchColliderHeight = 0.5f;
        [SerializeField] private float standingColliderOffset = 0.5f;
        [SerializeField] private float crouchColliderOffset = 0.25f;
        [SerializeField] private float colliderTransitionSpeed = 8f; // Speed of collider transition

        private Rigidbody2D _rigidbody;
        private BoxCollider2D _collider;
        private MovementVariablesSO _movementVars;
        private bool _isGrounded;
        private bool _isOnPlatform;
        private bool _isCrouching;
        private bool _isJumping;
        private Vector2 _moveDirection;
        private float _currentSpeed;
        private float _targetSpeed;
        private bool _hasInput;
        private float _currentAcceleration;
        private float _currentDeceleration;
        private float _currentJumpForce;
        private float _currentJumpHoldTime;
        private Vector2 _aimDirection = Vector2.right;
        private float _currentAimAngle;
        private float _targetAimAngle;
        private bool _jumpRequested;
        private float _requestedJumpForce;
        private bool _jumpHeldRequested;
        private float _requestedJumpHeldForce;
        private float _requestedJumpHeldScaling;
        private float _requestedJumpHeldHoldTime;
        private float _requestedJumpHeldMaxHoldTime;
        private ContactPoint2D[] _contacts = new ContactPoint2D[4];
        private float _lastVelocityY;
        private bool _wasMovingUp;
        private Vector2 _lastHorizontalMoveDirection = Vector2.right; // Default to right
        private bool _wasGrounded;
        private bool _wasCrouching;
        private bool _wasJumping;
        private bool _wasDead;
        private Vector2 _lastLoggedAimDirection = Vector2.zero;

        // Smooth collider transition variables
        private float _targetColliderHeight;
        private float _targetColliderOffset;
        private float _currentColliderHeight;
        private float _currentColliderOffset;
        private bool _isTransitioningCollider;

        // interface property implementations
        public bool IsGrounded => _isGrounded || _isOnPlatform;
        public bool IsJumping => _isJumping;
        public bool IsFlipping => false; // not used in platform style
        public bool IsMoving => _hasInput && Mathf.Abs(_currentSpeed) > 0.1f;
        public bool IsCrouching => _isCrouching;
        public float CurrentSpeed => _currentSpeed;
        public float JumpHoldTime => _currentJumpHoldTime;
        public Vector2 MoveDirection => _moveDirection;
        public Vector2 AimDirection => _aimDirection;
        public float AimAngle => _currentAimAngle;
        public bool CanFlip => false; // not used in platform style
        public Vector2 SnappedAimDirection => _aimDirection;
        public Vector2 LastHorizontalMoveDirection => _lastHorizontalMoveDirection;

        // Properties for external access
        public MovementVariablesSO MovementVariables
        {
            get => _movementVars;
            set => _movementVars = value;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();

            if (_movementVars == null)
            {
                Debug.LogWarning("MovementVariables not assigned to PilipalaPlatformHandler. Please assign via MovementVariables property.");
            }

            // setup rigidbody for proper platform interaction
            _rigidbody.gravityScale = 3f;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // ensure we're using the right physics material
            PhysicsMaterial2D material = new PhysicsMaterial2D();
            material.friction = 0;
            material.bounciness = 0f;
            _collider.sharedMaterial = material;

            // Initialize collider transition variables
            _currentColliderHeight = standingColliderHeight;
            _currentColliderOffset = standingColliderOffset;
            _targetColliderHeight = standingColliderHeight;
            _targetColliderOffset = standingColliderOffset;

            // set initial collider size
            SetColliderSize(false);
        }

        private void FixedUpdate()
        {
            _lastVelocityY = _rigidbody.linearVelocity.y;
            _wasMovingUp = _lastVelocityY > 0.1f;

            if (_jumpRequested)
            {
                Jump(_requestedJumpForce);
                _jumpRequested = false;
            }

            if (_jumpHeldRequested)
            {
                JumpHeld(_requestedJumpHeldForce, _requestedJumpHeldScaling, _requestedJumpHeldHoldTime, _requestedJumpHeldMaxHoldTime);
                _jumpHeldRequested = false;
            }

            // Update collider transition
            UpdateColliderTransition();
        }

        private void Update()
        {
            CheckGroundAndPlatforms();
            HandleMovement();
            UpdateAimDirection();
        }

        private void UpdateColliderTransition()
        {
            if (_isTransitioningCollider)
            {
                // Smoothly transition height and offset
                _currentColliderHeight = Mathf.Lerp(_currentColliderHeight, _targetColliderHeight, Time.fixedDeltaTime * colliderTransitionSpeed);
                _currentColliderOffset = Mathf.Lerp(_currentColliderOffset, _targetColliderOffset, Time.fixedDeltaTime * colliderTransitionSpeed);

                // Apply the transition
                Vector2 size = _collider.size;
                Vector2 offset = _collider.offset;
                size.y = _currentColliderHeight;
                offset.y = _currentColliderOffset;
                _collider.size = size;
                _collider.offset = offset;

                // Check if transition is complete
                if (Mathf.Abs(_currentColliderHeight - _targetColliderHeight) < 0.01f &&
                    Mathf.Abs(_currentColliderOffset - _targetColliderOffset) < 0.01f)
                {
                    _isTransitioningCollider = false;
                    _currentColliderHeight = _targetColliderHeight;
                    _currentColliderOffset = _targetColliderOffset;
                }
            }
        }

        private void CheckGroundAndPlatforms()
        {
            Vector2 checkPosition = (Vector2)transform.position + new Vector2(0, -_collider.bounds.extents.y);
            LayerMask combinedLayers = groundLayer | platformLayer;
            _isGrounded = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, combinedLayers);
            _isOnPlatform = _isGrounded && Physics2D.OverlapCircle(checkPosition, groundCheckRadius, platformLayer);
        }

        private void HandleMovement()
        {
            float targetSpeed = 0f;
            float accelRate = _currentAcceleration;

            if (_hasInput)
            {
                targetSpeed = _isCrouching && _movementVars != null ? _movementVars.crouchMoveSpeed : _targetSpeed;
            }
            else
            {
                targetSpeed = 0f;
                accelRate = _currentDeceleration;
            }

            // use lerp for snappier movement
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * accelRate);

            Vector2 velocity = _rigidbody.linearVelocity;
            velocity.x = _moveDirection.x * _currentSpeed;
            _rigidbody.linearVelocity = velocity;

            // Track last horizontal move direction (any time moving horizontally)
            if (Mathf.Abs(_moveDirection.x) > 0.01f)
            {
                _lastHorizontalMoveDirection = new Vector2(Mathf.Sign(_moveDirection.x), 0f);
            }

            if (!_hasInput && Mathf.Abs(_currentSpeed) < 0.1f)
            {
                _moveDirection = Vector2.zero;
            }
        }

        private void UpdateAimDirection()
        {
            // Snap aim to nearest 8 directions (NES Platform style)
            if (_moveDirection != Vector2.zero)
            {
                _aimDirection = SnapTo8Directions(_moveDirection);
            }
            // _currentAimAngle is not used for shooting, but can be kept for animation
            _currentAimAngle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;
        }

        // Helper: Snap a vector to the nearest 8 directions
        public static Vector2 SnapTo8Directions(Vector2 input)
        {
            if (input == Vector2.zero) return Vector2.right;
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            // 8-way: 0, 45, 90, 135, 180, -135, -90, -45
            float[] snapAngles = { 0, 45, 90, 135, 180, -135, -90, -45 };
            float minDiff = 360f;
            float snapped = 0f;
            foreach (float a in snapAngles)
            {
                float diff = Mathf.Abs(Mathf.DeltaAngle(angle, a));
                if (diff < minDiff)
                {
                    minDiff = diff;
                    snapped = a;
                }
            }
            float rad = snapped * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
        }

        public void Move(Vector2 direction, float speed, float acceleration, float deceleration, float maxSpeed)
        {
            _hasInput = direction != Vector2.zero;
            _currentAcceleration = acceleration;
            _currentDeceleration = deceleration;

            if (_hasInput)
            {
                _moveDirection = direction.normalized;
                _targetSpeed = maxSpeed;
            }
        }

        public void SetAimDirection(Vector2 direction)
        {
            if (direction == Vector2.zero) return;
            Vector2 newAim = direction.normalized;
            if (newAim != _lastLoggedAimDirection)
            {
                _lastLoggedAimDirection = newAim;
            }
            _aimDirection = newAim;
        }

        public void SetCrouchState(bool isCrouching)
        {
            if (_isCrouching == isCrouching) return;

            _isCrouching = isCrouching;
            StartColliderTransition(isCrouching);
        }

        private void StartColliderTransition(bool isCrouching)
        {
            if (isCrouching)
            {
                // When crouching: shrink from top down (offset moves down, height shrinks)
                _targetColliderHeight = crouchColliderHeight;
                _targetColliderOffset = crouchColliderOffset;
            }
            else
            {
                // When standing: grow from bottom up (offset moves up, height grows)
                _targetColliderHeight = standingColliderHeight;
                _targetColliderOffset = standingColliderOffset;
            }

            _isTransitioningCollider = true;
        }

        private void SetColliderSize(bool isCrouching)
        {
            Vector2 size = _collider.size;
            Vector2 offset = _collider.offset;

            if (isCrouching)
            {
                size.y = crouchColliderHeight;
                offset.y = crouchColliderOffset;
            }
            else
            {
                size.y = standingColliderHeight;
                offset.y = standingColliderOffset;
            }

            _collider.size = size;
            _collider.offset = offset;
        }

        public void Jump(float jumpForce)
        {
            if (_isGrounded && !_isCrouching)
            {
                _currentJumpForce = jumpForce;
                _isJumping = true;
                _currentJumpHoldTime = 0f;
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0);
                _rigidbody.AddForce(new Vector2(0, _currentJumpForce), ForceMode2D.Impulse);
            }
        }

        public void JumpHeld(float jumpForce, float scaling, float currentHoldTime, float maxHoldTime)
        {
            if (_rigidbody.linearVelocity.y > 0 && currentHoldTime < maxHoldTime)
            {
                _currentJumpHoldTime = currentHoldTime;
                float heldJumpForce = jumpForce * 0.02f;
                float adjustedScaling = Mathf.Lerp(0.8f, 0.1f, scaling);
                float additionalForce = heldJumpForce * adjustedScaling;
                _rigidbody.AddForce(new Vector2(0, additionalForce), ForceMode2D.Impulse);
            }
        }

        public Vector2 GetPosition() => transform.position;
        public Quaternion GetRotation() => Quaternion.identity;
        public Vector2 GetVelocity() => _rigidbody.linearVelocity;

        public void RequestJump(float jumpForce)
        {
            _jumpRequested = true;
            _requestedJumpForce = jumpForce;
        }

        public void RequestJumpHeld(float jumpForce, float scaling, float holdTime, float maxHoldTime)
        {
            _jumpHeldRequested = true;
            _requestedJumpHeldForce = jumpForce;
            _requestedJumpHeldScaling = scaling;
            _requestedJumpHeldHoldTime = holdTime;
            _requestedJumpHeldMaxHoldTime = maxHoldTime;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & platformLayer) != 0)
            {
                Physics2D.IgnoreCollision(_collider, collision.collider, false);
            }
        }
    }
}