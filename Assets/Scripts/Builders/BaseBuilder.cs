using UnityEngine;

public class BaseBuilder : MonoBehaviour
{
    [SerializeField] private Base _basePrefab;

    public Base CreateNewBase(Robot initialRobot)
    {
        Base createdBase = Instantiate(_basePrefab);

        createdBase.Initialize(initialRobot);

        return createdBase;
    }
}
