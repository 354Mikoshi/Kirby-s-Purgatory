using UnityEngine;

public class AppleController : MonoBehaviour
{
    private void Start() {
    }

    private void Update() {
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "Player") {
            Destroy(gameObject);
        }
    }
}
