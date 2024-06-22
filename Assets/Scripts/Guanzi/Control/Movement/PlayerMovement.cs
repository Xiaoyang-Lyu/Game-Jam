using System;
using UnityEngine;

namespace GrayCity.Control.Movement._Scripts
{
    // 需要添加Rigidbody2D和Capsule Collider2D组件
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerMovement : MonoBehaviour, IPlayerMovement
    {
        private ControllerInput _input;
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput; //当前帧的输入
        private Vector2 _frameVelocity; //当前帧的速度
        private bool _cachedQueryStartInColliders;  //缓存的Physics2D.queriesStartInColliders，用于在检测碰撞时忽略自身


        private bool _jumpDown;
        private bool _jumpHeld;
        private Vector2 _move;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            
            InitInput();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;   //缓存Physics2D.queriesStartInColliders的初始值
        }

        private void InitInput()
        {
            _input = new ControllerInput();
            
            _input.Player.Movement.performed += ctx => _move = CanMove() ? ctx.ReadValue<Vector2>() : Vector2.zero;
            _input.Player.Movement.canceled += ctx => _move = Vector2.zero;
            
            _input.Player.Jump.performed += ctx => _jumpHeld = CanMove() ? true : false;
            _input.Player.Jump.canceled += ctx => _jumpHeld = false;
            _input.Player.Jump.started += ctx => _jumpDown = CanMove() ? true : false;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            HandleInput();
        }

        private void OnEnable()
        {
            _input.Enable();
        }
        
        private void OnDisable()
        {
            _input.Disable();
        }

        private void HandleInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = _jumpDown,
                JumpHeld = _jumpHeld,
                Move = _move
            };
            
            _jumpDown = false;

            if (_stats.SnapInput)   //如果启用了输入捕捉，则检测死区，并使用Sign函数将输入转换为-1、0、1
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;  //_jumpToConsume用于标记是否需要在下一个FixedUpdate中执行跳跃
                _timeJumpWasPressed = _time;    //记录跳跃按下的时间
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();  //检测碰撞

            HandleJump();   //处理跳跃
            HandleDirection();  //处理水平方向
            HandleGravity();    //处理重力
            ApplyMovement();    //应用速度
        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;
            
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;  // 设为 false 来忽略 trigger 碰撞体
            contactFilter.SetLayerMask(~_stats.PlayerLayer);
            RaycastHit2D[] results = new RaycastHit2D[1];
            
            // 检测是否与地面和天花板碰撞
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, contactFilter, results, _stats.GrounderDistance) > 0;
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, contactFilter, results, _stats.GrounderDistance) > 0;

            // 如果碰到天花板，将垂直速度设为0
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // 如果碰到地面，将垂直速度设为0
            if (!_grounded && groundHit)
            {
                _grounded = true;   //标记为在地面上
                _coyoteUsable = true;   //标记为可使用coyote time，这个用于在离开地面后一段时间内仍然可以跳跃
                _bufferedJumpUsable = true;  //标记为可使用buffered jump，这个用于缓存跳跃，使得即使在落地前按下跳跃键也能在落地后立即跳跃
                _endedJumpEarly = false;    //重置是否提前结束跳跃
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));   //触发GroundedChanged事件
            }
            // 离开地面的瞬间
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;   //恢复Physics2D.queriesStartInColliders的初始值
        }

        #endregion


        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed = -10.0f;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;   //判断是否有缓存的跳跃，_stats.JumpBuffer最长为缓存时间
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            // 如果当前还没有提前结束跳跃，且在空中，且松开跳跃键，且垂直速度大于0，则标记为提前结束跳跃
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;
            
            // 既没有跳跃，也没有缓存的跳跃，这两者实际上是一个东西，都在第56行
            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;    //给_frameVelocity.y赋值为跳跃力
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)    //如果没有输入
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;  //根据是否在地面上选择减速度
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);  //根据减速度减速
            }
            else   //如果有输入，注意下方代码是唯一的给_frameVelocity.x赋值的地方
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement() => _rb.velocity = _frameVelocity;  //将速度赋值给Rigidbody2D

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif

        private bool CanMove()
        {
            return true;
        }
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerMovement
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}