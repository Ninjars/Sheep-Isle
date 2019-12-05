using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    public Vector3 orbitTarget;
    public Vector2 offset;
    public float orbitSpeed;

    private Camera cam;

    void Awake() {
        cam = GetComponent<Camera>();
        var initialAngle = UnityEngine.Random.Range(0, 360);
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
