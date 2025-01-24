using System;
using System.Collections;
using UnityEngine;
using AllanReford._SimplePlatformer.Code.Managers;
using Unity.VisualScripting;

namespace AllanReford._SimplePlatformer.Code.Player
{
    public class PlayerController : MonoBehaviour
    {

        public float speed = 5;
        [Range(1,10)]
        public float jumpVelocity;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;
        
        [Range(0,1)]
        public float colliderResetTime = 0.25f;

        public Transform groundCheckObject;
        public LayerMask groundLayer;
        public float groundCheckRadius;
        private int _jumpCount;
        
        //wall stuff
        public LayerMask wallLayer;
        public Transform wallCheckObject;
        
        private bool _isWallSliding;
        private float _wallSlideSpeed = 1f;

        private bool _isWallJumping;
        private float _wallJumpingDirection;
        private float _wallJumpingTime = 0.2f;
        private float _wallJumpingCounter;
        private float _wallJumpingDuration = 0.4f;
        private Vector2 _wallJumpingPower = new Vector2(8f, 8f);
        
        
        
        private bool _isGrounded;
        private bool _isFacingRight = true;

        private float _horizontalInput;
        
        private Rigidbody2D _rb;
        private BoxCollider2D _boxCollider;
        private SpriteRenderer _spriteRenderer;
        private void Start()
        {
            _rb = GetComponentInChildren<Rigidbody2D>();
            _boxCollider = GetComponentInChildren<BoxCollider2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Manager.Instance.InputManager.OnJumpPerformedEvent += Jump;
            
        }

        private void Update()
        {
            _horizontalInput = Manager.Instance.InputManager.wasdInput.x;
            
            if (_rb.linearVelocity.y < 0)
            {
                _rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            }else if (_rb.linearVelocity.y > 0 && !Manager.Instance.InputManager.IsJumpButtonDown())
            {
                _rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            }
            
            if (Manager.Instance.InputManager.wasdInput.y < 0)
            { 
                if (Physics2D.OverlapCircle(groundCheckObject.position, groundCheckRadius, LayerMask.GetMask("Ledge")))
                {
                    _boxCollider.enabled = false;
                    StartCoroutine(ReEnableCollider());
                }
            }
            
            _isGrounded = Physics2D.OverlapCircle(groundCheckObject.position, groundCheckRadius, groundLayer);
            
            if(_isGrounded)
                _jumpCount = 0;
            
            Debug.Log(IsWalled());
            
            WallSlide();
            WallJump();
            
            if(!_isWallJumping)
                Flip();
        }

        private void FixedUpdate()
        {
            if(!IsWalled())
             _rb.linearVelocity = new Vector2(_horizontalInput * speed, _rb.linearVelocity.y);
        }
        
        private void Jump()
        {
            if (_isGrounded || _jumpCount < 1)
            {
                _rb.linearVelocity = Vector2.up * jumpVelocity;
                _jumpCount++;
            }

            if (_wallJumpingCounter > 0)
            {
                _isWallJumping = true;
                _rb.linearVelocity = new Vector2(_wallJumpingDirection * _wallJumpingPower.x, _wallJumpingPower.y);
                _wallJumpingCounter = 0f;
                
                if(transform.localScale.x !=  _wallJumpingDirection)
                    Flip();
                
                Invoke(nameof(StopWallJumping), _wallJumpingDuration);
            }
        }

        private bool IsWalled()
        {
            return Physics2D.OverlapCircle(wallCheckObject.position, 0.2f, wallLayer);
        }
        
        private void WallSlide()
        {
            if (IsWalled() && _isGrounded == false && _horizontalInput != 0f)
            {
                Debug.Log("Wall sliding");
                _isWallSliding = true;
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Clamp(_rb.linearVelocity.y, -_wallSlideSpeed, float.MaxValue));
            }
            else
            {
                _isWallSliding = false;
            }
        }

        private void WallJump()
        {
            if (_isWallSliding)
            {
                _isWallJumping = false;
                _wallJumpingDirection = -transform.localScale.x;
                _wallJumpingCounter = _wallJumpingTime;
                
                CancelInvoke(nameof(StopWallJumping));
            }
            else
            {
                _wallJumpingCounter -= Time.deltaTime;
            }
        }

        private void StopWallJumping()
        {
            _isWallJumping = false;
        }
        
        private void Flip()
        {
            if(_isFacingRight && _horizontalInput < 0 || !_isFacingRight && _horizontalInput > 0)
            {
                _isFacingRight = !_isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }
        }
        
        IEnumerator ReEnableCollider()
        {
            _boxCollider.enabled = false;
            yield return new WaitForSeconds(colliderResetTime);
            _boxCollider.enabled = true;
        }
        
        private void OnDrawGizmos()
        {
            // Draw the ground check circle in the Scene view for visualization
            if (groundCheckObject != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheckObject.position, groundCheckRadius);
            }
        }
    }
}
