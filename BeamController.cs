using UnityEngine;

public class BeamController : MonoBehaviour
{
    public AudioClip kirbyGetsDamagedSound;

    private GameObject player;
    private float stageSize;
    private float exist_time, disappear_time;
    private AudioSource audioSource;

    private void Start()
    {
        player = GameObject.Find("kirby_prefab");
        audioSource = GetComponent<AudioSource>();
        transform.LookAt(player.transform);

        stageSize = 10f;
        exist_time = 0f;
        disappear_time = 7f;
    }

    private void Update()
    {
        //if (transform.position.x < -stageSize || transform.position.x > stageSize || transform.position.y > 15
        //    || transform.position.z < -stageSize || transform.position.z > stageSize) {
        //    Destroy(gameObject);
        //}

        exist_time += Time.deltaTime;
        if (exist_time > disappear_time) {
            Destroy(gameObject);
        }
    }

    void OnParticleCollision(GameObject obj) {
        if (obj.tag == "Player") {
            audioSource.PlayOneShot(kirbyGetsDamagedSound);
            PlayerController.strength -= 1;
        }
    }
}
