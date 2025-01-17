using UnityEngine;
using AllanReford._SimplePlatformer.Code.Managers;

namespace AllanReford._SimplePlatformer.Code.Player
{
    public class PlayerController : MonoBehaviour
    {

        public float speed = 5;
        [Range(1,10)]
        public float jumpVelocity;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;

        public Transform groundCheckObject;
        public LayerMask groundLayer;
        public float groundCheckRadius;
        
        [SerializeField]private bool _isGrounded;
        
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private void Start()
        {
            _rb = GetComponentInChildren<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Manager.Instance.InputManager.OnJumpPerformedEvent += Jump;
        }

        private void Update()
        {
            if (_rb.linearVelocity.y < 0)
            {
                _rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            }else if (_rb.linearVelocity.y > 0 && !Manager.Instance.InputManager.IsJumpButtonDown())
            {
                _rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            }

            if (Manager.Instance.InputManager.horizontalInput.x < 0)
            {
                _spriteRenderer.flipX = true;
                transform.Translate(Vector3.left * (speed * Time.deltaTime));
            }
            
            if (Manager.Instance.InputManager.horizontalInput.x > 0)
            {
                _spriteRenderer.flipX = false;
                transform.Translate(Vector3.right * (speed * Time.deltaTime));
            }
            
            _isGrounded = Physics2D.OverlapCircle(groundCheckObject.position, groundCheckRadius, groundLayer);
            
        }

        private void Jump()
        {
            if( _isGrounded)
                _rb.linearVelocity = Vector2.up * jumpVelocity;
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
