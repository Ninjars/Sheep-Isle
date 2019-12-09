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
        private float baseBabyScale = 0.33f;
        private AutonomousLegomatic legController;
        public GameObject babySpawnParticleEffect;
        public GameObject happySheepParticleEffect;

        void Awake() {
            base.init();
            legController = GetComponent<AutonomousLegomatic>();
            initialObjPos = body.localPosition;
        }

        void Start() {
            moveToRandomPoint();
            updateScale();
        }

        void Update() {
            updateBreathing();

            if (foodTarget == null) {
                currentDelayTime += Time.deltaTime;
                if (currentDelayTime > nextActionDelay) {
                    moveToRandomPoint();
                    currentDelayTime = 0;
                }
            }
        }

        public bool setFoodTarget(Food target) {
            if (foodTarget == null) {
                currentDelayTime = 0;
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
            if (foodEaten > foodCountToReproduce / 1.5) {
                GameObject.Instantiate(happySheepParticleEffect, transform.position + Vector3.up, Quaternion.identity);
            }

            if (foodEaten >= foodCountToReproduce) {
                foodEaten -= foodCountToReproduce;

                for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                    var position = Game.Utils.RandomNavSphere(transform.position, 3f, -1);
                    if (position.x == Mathf.Infinity) continue;
                    GameObject.Instantiate(babySpawnParticleEffect, position, Quaternion.identity);
                    var newSheep = GameObject.Instantiate(this, position, UnityEngine.Random.rotation);
                    newSheep.foodEaten = babyFoodCount;
                }

            } else if (foodEaten <= 0) {
                updateScale();
            }
        }

        private void updateScale() {
            float scale = 1 - (foodEaten / (float)babyFoodCount);
            scale = baseBabyScale + (1 - baseBabyScale) * scale;
            transform.localScale = new Vector3(scale, scale, scale);
            legController.scaleFeet(transform.localScale);
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
            nextActionDelay = UnityEngine.Random.Range(minActionDelaySeconds, maxActionDelaySeconds);
            MoveToLocation(targetPosition);
        }
    }
}
