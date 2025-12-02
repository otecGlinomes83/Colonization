public struct ResourceParameters
{
    public ResourceType Type;

    private int _currentCount;
    private int _expectedCount;

    public int CurrentCount => _currentCount;
    public int ExpectedCount => _expectedCount;

    public ResourceParameters(ResourceType type, int currentCount, int expectedCount = 0)
    {
        Type = type;

        _currentCount = currentCount;
        _expectedCount = expectedCount;
    }

    public void IncreaseExpectedCount()
    {
        _expectedCount++;
    }

    public void IncreaseCount()
    {
        _currentCount++;
    }

    public void DecreaseCount(int count)
    {
        _currentCount -= count;
    }

    public void DecreaseExpectedCount()
    {
        _expectedCount--;
    }
}