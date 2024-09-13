using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10;
    void Update()
    {
        //transform.position = Vector2.MoveTowards(transform.position, transform.forward*10,speed*Time.deltaTime);
        transform.position += transform.right * speed * Time.deltaTime*-1;
        if (speed < 0.2f) Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.right*-10);
    }
}