using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    public Transform player;
    public ParticleSystem gunfireVfx;
    public AudioSource gunfireSfx;
    public float shootInterval;

    private float _timer;
    
    private void Update()
    {
        if (player != null)
        {
            gameObject.transform.LookAt(player);
            _timer += Time.deltaTime;
            if (_timer >= shootInterval)
            {
                gunfireSfx.Play();
                gunfireVfx.Play();
                _timer = 0f;
            }
        }
    }

}
