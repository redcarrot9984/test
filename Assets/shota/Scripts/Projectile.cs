using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private int damage;
    public float speed = 10f;

    public void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void Update()
    {
        if (target != null)
        {
            // ターゲットに向かって移動
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            // ターゲットがいない場合は弾を消す
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 敵に命中した場合
        if (other.transform == target)
        {
            target.GetComponent<Unit>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}