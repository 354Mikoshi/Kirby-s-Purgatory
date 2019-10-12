using UnityEngine;

public class ChargedBulletController : MonoBehaviour {
    private void Start() {
        Destroy(gameObject, 11);
        transform.rotation = Camera.main.transform.rotation;
    }

    private void Update() {
    }

    void OnParticleCollision(GameObject obj) {
        if (obj.gameObject.tag == "Kyuchan") {
            obj.GetComponent<KyuchanController>().strength -= 100;
            obj.GetComponent<KyuchanController>().DisplayDamageText(100);
        }
    }
}
