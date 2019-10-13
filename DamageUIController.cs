using UnityEngine;
using UnityEngine.UI;

public class DamageUIController : MonoBehaviour {

    private Text damageText;
    private float fadeOutSpeed = 1f;
    private float elapsedTime;

    private float moveSpeed = 6f;

    private void Start() {
        elapsedTime = 0f;
        Destroy(gameObject, 1);
        damageText = gameObject.GetComponent<Text>();
    }

    private void Update() {
        elapsedTime += Time.deltaTime;
        transform.rotation = Camera.main.transform.rotation;
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}
