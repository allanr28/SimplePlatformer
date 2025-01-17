using UnityEngine;
using AllanReford._SimplePlatformer.Code.Input;

namespace AllanReford._SimplePlatformer.Code.Managers
{
    public class Manager : MonoBehaviour
    {
        public static Manager Instance;
        
        public InputManager InputManager { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            
            Instance = this;

            InputManager = GetComponent<InputManager>();
        }
    }
}