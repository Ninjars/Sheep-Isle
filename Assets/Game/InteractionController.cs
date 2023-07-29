using UnityEngine;

namespace Game {
    public class InteractionController : MonoBehaviour {
        public LayerMask groundLayerMask;
        public LayerMask sheepLayerMask;
        public GameObject placedObject;

        private float inputStartTime;
        private Vector2 inputStartPos;
        private bool isMouseInputDown = false;

        void Update() {
            if (!isMouseInputDown && Input.GetButtonDown("Fire1")) {
                isMouseInputDown = true;
                inputStartTime = Time.time;
                inputStartPos = Input.mousePosition;

            } else if (isMouseInputDown && Input.GetButtonUp("Fire1")) {
                isMouseInputDown = false;
                if (Time.time - inputStartTime < 0.1f) {
                    interactAtPosition(inputStartPos, Input.mousePosition);
                }
            }
        }

        private void interactAtPosition(Vector2 startPosition, Vector2 endPosition) {
            if ((endPosition - startPosition).sqrMagnitude > 1000) return;

            Ray ray = Camera.main.ScreenPointToRay(startPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, sheepLayerMask)) {
                var sheep = hit.transform.gameObject.GetComponent<SheepAgent>();
                if (sheep != null) {
                    sheep.baa();
                }

            } else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask)) {
                var position = hit.point;
                Instantiate(placedObject, position, Quaternion.identity);
            }
        }
    }
}
