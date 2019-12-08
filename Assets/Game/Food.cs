using UnityEngine;

public class Food : MonoBehaviour {

    public Transform bouncingObject;
    public float bounceHeight = 0.5f;

    private Vector3 initialObjPos;
    private float timer;

    void Start() {
        initialObjPos = bouncingObject.position;
        timer = 0;
    }

    void Update() {
        timer += Time.deltaTime;
        timer = timer % (2 * Mathf.PI);
        bouncingObject.transform.position = new Vector3(initialObjPos.x, initialObjPos.y + bounceHeight * Mathf.Sin(timer), initialObjPos.z);
    }
}
