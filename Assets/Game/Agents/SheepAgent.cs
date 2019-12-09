using UnityEngine;

namespace Game {
    public class SheepAgent : BaseAgent {
        private float currentDelayTime;
        private float nextActionDelay;

        public float wanderDistance = 8;
        public float minActionDelaySeconds = 3;
        public float maxActionDelaySeconds = 10;
        public float breathCycleTime = 2;
        public Transform body;
        public float breathHeight = 0.025f;
        private Food foodTarget;
        private Vector3 initialObjPos;
        private float breathTimer = -1;
        private float breathCycleCurrent;
        private int foodEaten;
        public int foodCountToReproduce = 5;
        public int babyFoodCount = -5;

        void Awake() {
            base.init();
            initialObjPos = body.localPosition;
        }

        void Start() {
            moveToRandomPoint();
        }

        void Update() {
            updateBreathing();

            if (foodTarget != null) {
                // check to see if can eat the food
                currentDelayTime = 0;

            } else {
                currentDelayTime += Time.deltaTime;
                if (currentDelayTime > nextActionDelay) {
                    moveToRandomPoint();
                    currentDelayTime = 0;
                }
            }
        }

        public bool setFoodTarget(Food target) {
            if (foodTarget == null) {
                foodTarget = target;
                MoveToLocation(target.transform.position);
                return true;
            } else {
                return false;
            }
        }

        public void onFoodEaten(Food food) {
            if (food == foodTarget) {
                foodTarget = null;
            }

            foodEaten++;
            // TODO: happy particle effect

            if (foodEaten >= foodCountToReproduce) {
                foodEaten -= foodCountToReproduce;
                // TODO: spawn babbies
            }
        }

        private void updateBreathing() {
            breathTimer -= Time.deltaTime;
            if (breathTimer < 0) {
                breathCycleCurrent = breathCycleTime * UnityEngine.Random.Range(0.8f, 1.2f);
                breathTimer = breathCycleCurrent;
            }
            var fraction = 1 - breathTimer / breathCycleCurrent;
            var value = fraction * (2 * Mathf.PI);
            body.localPosition = new Vector3(initialObjPos.x, initialObjPos.y + breathHeight * Mathf.Sin(value), initialObjPos.z);
        }

        private void moveToRandomPoint() {
            Vector3 targetPosition = Game.Utils.RandomNavSphere(transform.position, wanderDistance, -1);
            nextActionDelay = (float)UnityEngine.Random.Range(minActionDelaySeconds, maxActionDelaySeconds);
            MoveToLocation(targetPosition);
        }
    }
}
