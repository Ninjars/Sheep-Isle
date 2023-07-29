using UnityEngine;

namespace Game {
    public class TouchOrbiter : MonoBehaviour {

        public Transform target;
        public float distance = 5.0f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;
        public float zoomSpeed = 10f;

        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        public float distanceMin = .5f;
        public float distanceMax = 15f;

        private float initialPinchInput = -1;
        private float initialPinchDistance = -1;
        private bool isActive;

        float x = 0.0f;
        float y = 0.0f;

        void Start() {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }
        
        void Update() {
            switch (Input.touchCount) {
                case 1:
                Debug.Log("handling 1 touch input");
                    initialPinchInput = -1;
                    isActive = true;
                    updateTargetPosition(Input.GetTouch(0).deltaPosition);
                    break;  
                case 2:
                Debug.Log("handling 2 touch input");
                    updateTargetPosition(Input.GetTouch(0).deltaPosition * 0.5f + Input.GetTouch(1).deltaPosition * 0.5f);
                    isActive = true;
                    if (initialPinchInput < 0) {
                        initialPinchInput = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
                        initialPinchDistance = distance;
                    } else {
                        updateTargetDistance(initialPinchInput, (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude);
                    }
                    break;
                default:
                    initialPinchInput = -1;
                    break;
            }
        }

        private void updateTargetDistance(float initialInput, float currentInput) {
            var targetDistance = initialPinchDistance * (initialInput / currentInput);
            distance = Mathf.Clamp(targetDistance, distanceMin, distanceMax);
        }

        private void updateTargetPosition(Vector2 inputOffset) {
            Debug.Log("updateTargetPosition " + inputOffset);
            x += inputOffset.x * xSpeed * distance * 0.02f;
            y -= inputOffset.y * ySpeed * 0.02f;
            y = ClampAngle(y, yMinLimit, yMaxLimit);
        }

        void LateUpdate() {
            if (target && isActive) {
                Quaternion rotation = Quaternion.Euler(y, x, 0);
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rotation, 0.1f);
                transform.position = Vector3.Slerp(transform.position, position, 0.1f);
            }
        }

        private static float ClampAngle(float angle, float min, float max) {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}