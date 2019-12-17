using System;
using UnityEngine;

namespace Game {
    public class Food : MonoBehaviour {

        public GameObject onDestroyEffect;
        public Transform bouncingObject;
        public float bounceHeight = 0.5f;
        public float life = 60f;

        private Vector3 initialObjPos;
        private float bounceTimer;

        void Start() {
            initialObjPos = bouncingObject.position;
            bounceTimer = 0;
            SheepAgent[] sheep = FindObjectsOfType<SheepAgent>();

            if (sheep.Length == 0) GameObject.Destroy(gameObject);

            Array.Sort(sheep, new Comparison<SheepAgent>((a, b) =>
                (transform.position - a.transform.position).sqrMagnitude
                    .CompareTo((transform.position - b.transform.position).sqrMagnitude))
            );
            bool wasSet = false;
            for (int i = 0; i < sheep.Length; i++) {
                wasSet = sheep[i].setFoodTarget(this);
                if (wasSet) break;
            }
        }

        private void OnTriggerEnter(Collider other) {
            var sheep = other.gameObject.GetComponent<SheepAgent>();
            if (sheep == null) return;
            sheep.onFoodEaten(this);
            GameObject.Instantiate(onDestroyEffect, transform.position, Quaternion.identity);
            GameObject.Destroy(gameObject);
        }   

        void Update() {
            life -= Time.deltaTime;
            if (life < 0) {
                GameObject.Destroy(gameObject);
                return;
            }

            bounceTimer += Time.deltaTime;
            bounceTimer = bounceTimer % (2 * Mathf.PI);
            bouncingObject.transform.position = new Vector3(initialObjPos.x, initialObjPos.y + bounceHeight * Mathf.Sin(bounceTimer), initialObjPos.z);
        }
    }
}
