using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceType _type;

    public event Action<Resource> ReadyForRelease;

    public ResourceType Type => _type;
    public Rigidbody Rigidbody { get; private set; }
    public bool IsReserved { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Release() =>
        ReadyForRelease?.Invoke(this);

    public void SetReserve(bool newState) =>
        IsReserved = newState;
}
