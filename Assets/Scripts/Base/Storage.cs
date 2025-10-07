using System.Collections.Generic;
using System.Linq;

public class Storage
{
    private bool _isFull = false;

    private int _metalCount;
    private int _plasticCount;
    private int _wiresCount;

    private int _maxMetalCount = 3;
    private int _maxPlasticCount = 3;
    private int _maxWiresCount = 3;

    private int _sumResources;
    private int _maxResourcesCount;

    private Dictionary<ResourceType, int> _resourceParameters;

    public bool IsFull => _isFull;

    public Storage()
    {
        _maxResourcesCount = _maxMetalCount + _maxPlasticCount + _maxWiresCount;

        _resourceParameters = new Dictionary<ResourceType, int>
        {
            {ResourceType.Metal,_metalCount},
            {ResourceType.Plastic,_plasticCount},
            {ResourceType.Wires,_wiresCount}
        };
    }

    public void AddResource()
    {
        _sumResources = _wiresCount + _metalCount + _plasticCount;

        if (_sumResources >= _maxResourcesCount)
        {
            _isFull = true;
        }
    }

    public ResourceType GetNecessaryResourceType() =>
            _resourceParameters.OrderBy(x => x.Value).First().Key;

}
