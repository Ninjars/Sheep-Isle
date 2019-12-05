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
		public bool debugDraw = false;

		void Awake() {
			base.init();
		}

		// Use this for initialization
		void Start () {
			moveToRandomPoint();
		}
		
		// Update is called once per frame
		void Update () {
			currentDelayTime += Time.deltaTime;
			if (currentDelayTime > nextActionDelay) {
                moveToRandomPoint();
			    currentDelayTime = 0;
			}
		}

		private void moveToRandomPoint() {
            Vector3 targetPosition = Game.Utils.RandomNavSphere(transform.position, wanderDistance, -1);
			nextActionDelay = (float)UnityEngine.Random.Range(minActionDelaySeconds, maxActionDelaySeconds);
			MoveToLocation(targetPosition);
		}
    }
}
