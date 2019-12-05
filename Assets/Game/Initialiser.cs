using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialiser : MonoBehaviour {

    public GameObject sheepPrefab;
    public int sheepCount = 20;
    public float spawnRadius = 20;
    
    void Start() {
        for (int i = 0; i < sheepCount; i++) {
            var position = Game.Utils.RandomNavSphere(transform.position, spawnRadius, -1);
            GameObject.Instantiate(sheepPrefab, position, UnityEngine.Random.rotation);
        }
    }
}
