using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AllanReford._SimplePlatformer.Code.Input
{
    public class InputManager : MonoBehaviour
    {
        public event Action OnJumpPerformedEvent;

        public bool isJumpButtonDown;
        
        public Vector2 wasdInput;

        private PlayerInput _playerInput;

        public bool IsJumpButtonDown()
        {
            return isJumpButtonDown;
        }
        
        
        private void Awake()
        {
            _playerInput = new PlayerInput();
    
        }

        private void Start()
        {
            isJumpButtonDown = false;
        }

        private void Update()
        {
            wasdInput = _playerInput.Locomotion.HorizontalMovement.ReadValue<Vector2>();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.Locomotion.Jump.started += ctx => isJumpButtonDown = true;
            _playerInput.Locomotion.Jump.performed += ctx => OnJumpPerformedEvent?.Invoke();
            _playerInput.Locomotion.Jump.canceled += ctx => isJumpButtonDown = false;
        }
        
        
    }
}
