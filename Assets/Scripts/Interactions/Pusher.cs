using UnityEngine;

public class Pusher : MonoBehaviour
{
    [SerializeField] private float _force;

    public void Push(Rigidbody rigidbody) =>
        rigidbody.AddForce((rigidbody.transform.position - transform.position).normalized * _force, ForceMode.Impulse);
}
