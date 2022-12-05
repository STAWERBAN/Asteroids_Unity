using System;
using Runtime.Data;
using Runtime.Interfaces;
using UnityEngine;

namespace Runtime.Services.Input
{
    public class InputService : IUpdatable
    {
        private readonly InputData _data;

        public event Action FireButtonPress = delegate {  };
        public event Action LaserButtonPress = delegate {  }; 

        public InputService(InputData data)
        {
            _data = data;
        }

        public void Update()
        {
            if(UnityEngine.Input.GetKey(_data.FireButton))
                FireButtonPress.Invoke();
            
            if(UnityEngine.Input.GetKeyDown(_data.LaserKey))
                LaserButtonPress.Invoke();
        }

        public Vector2 GetInputAxis()
        {
            var x = UnityEngine.Input.GetAxis("Horizontal");
            var y = UnityEngine.Input.GetAxis("Vertical");

            y = Mathf.Clamp(y, 0, 1);

            var inputDirection = new Vector2(x, y);
            
            return inputDirection;
        }
    }
}