using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceType _type;

    public event Action<Resource> ReadyForRelease;

    public Rigidbody Rigidbody { get; private set; }
    public ResourceType Type => _type;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Release() =>
        ReadyForRelease?.Invoke(this);
}
