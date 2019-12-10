using System.Linq;
using Doozy.Engine;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour {
        private string saveGameName = "sheepIsle";
        public SheepAgent sheepPrefab;
        public int initialSheepCount = 3;
        public float spawnRadius = 30;
        private float autosaveTimer;
        private float autosaveInterval = 120;

        void Start() {
            if (SaveGameSystem.DoesSaveGameExist(saveGameName)) {
                var success = loadIsland();
                if (!success) {
                    Debug.LogError("failed to load saved island");
                    spawnInitialSheep();
                }

            } else {
                spawnInitialSheep();
            }
        }

        private void Update() {
            autosaveTimer += Time.deltaTime;
            if (autosaveTimer > autosaveInterval) {
                autosaveTimer = 0;
                onSaveEvent();
            }

            if (Input.GetKey("escape")) {
                GameEventManager.ProcessGameEvent(new GameEventMessage("MENU"));
            }
        }

        private void spawnInitialSheep() {
            for (int i = 0; i < initialSheepCount; i++) {
                spawnSheep(0);
            }
        }

        private void spawnSheep(int foodLevel) {
            var position = Game.Utils.RandomNavSphere(transform.position, spawnRadius, -1);
            var sheep = GameObject.Instantiate(sheepPrefab, position, UnityEngine.Random.rotation);
            sheep.foodEaten = foodLevel;
        }

        public void onResetEvent() {
            SheepAgent[] allSheep = FindObjectsOfType<SheepAgent>();
            foreach (var sheep in allSheep) {
                GameObject.Destroy(sheep.gameObject);
            }
            Food[] allFood = FindObjectsOfType<Food>();
            foreach (var food in allFood) {
                GameObject.Destroy(food.gameObject);
            }
            spawnInitialSheep();
        }

        public void onSaveEvent() {
            var success = saveIsland();
            if (!success) {
                Debug.LogError("failed to save game!");
            } else {
                autosaveTimer = 0;
                Debug.Log("updated save");
            }
        }

        public void onExitEvent() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #region save functions
        private bool loadIsland() {
            var saveGame = SaveGameSystem.LoadGame(saveGameName);
            if (saveGame == null) {
                return false;
            }

            foreach (var value in saveGame.sheepLevels) {
                spawnSheep(value);
            }
            return true;
        }

        private bool saveIsland() {
            SheepAgent[] allSheep = FindObjectsOfType<SheepAgent>();
            int[] sheepLevels = allSheep.Select(sheep => sheep.foodEaten).ToArray();
            SaveGame saveGame = new SaveGame();
            saveGame.sheepLevels = sheepLevels;
            return SaveGameSystem.SaveGame(saveGame, saveGameName);
        }

        #endregion
    }
}
