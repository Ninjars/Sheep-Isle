using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game {
	public class WanderingAgent : BaseAgent {
		private float currentDelayTime;
		private float nextActionDelay;

        public float wanderDistance = 8;
		public float minActionDelaySeconds = 3;
		public float maxActionDelaySeconds = 10;
        public float breathCycleTime = 2;
        public Transform body;
        public float breathHeight = 0.05f;
        private Vector3 initialObjPos;
        private float breathTimer = -1;
        private float breathCycleCurrent;

		void Awake() {
			base.init();
            initialObjPos = body.localPosition;
		}

		void Start () {
			moveToRandomPoint();
		}
		
		void Update () {
            updateBreathing();
			currentDelayTime += Time.deltaTime;
			if (currentDelayTime > nextActionDelay) {
                moveToRandomPoint();
			    currentDelayTime = 0;
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
