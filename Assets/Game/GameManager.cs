using System;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour {
        private string saveGameName = "sheepIsle";
        public GameObject sheepPrefab;
        public int initialSheepCount = 3;
        public float spawnRadius = 30;

        void Start() {
            if (SaveGameSystem.DoesSaveGameExist(saveGameName)) {
                var success = loadIsland(SaveGameSystem.LoadGame(saveGameName));
            } else {
                spawnInitialSheep();
            }
        }

        private bool loadIsland(SaveGame saveGame) {
            // TODO: attempt to load up sheep, or return false if failure
            throw new NotImplementedException();
        }

        private void resetIsland() {
            SheepAgent[] allSheep = FindObjectsOfType<SheepAgent>();
            foreach(var sheep in allSheep) {
                GameObject.Destroy(sheep.gameObject);
            }
            Food[] allFood = FindObjectsOfType<Food>();
            foreach(var food in allFood) {
                GameObject.Destroy(food.gameObject);
            }
            spawnInitialSheep();
        }

        private void spawnInitialSheep() {
            for (int i = 0; i < initialSheepCount; i++) {
                var position = Game.Utils.RandomNavSphere(transform.position, spawnRadius, -1);
                GameObject.Instantiate(sheepPrefab, position, UnityEngine.Random.rotation);
            }
        }

        private void displayMenu() {

        }

        private void displayGame() {


        }
    }
}
