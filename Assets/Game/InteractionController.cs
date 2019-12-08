using UnityEngine;

public class InteractionController : MonoBehaviour {
    public LayerMask inputLayerMask;
    public GameObject placedObject;

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, inputLayerMask)) {
                var position = hit.point;
                Instantiate(placedObject, position, Quaternion.identity);
            }
        }
    }
}
