public struct ResourceParameters
{
    public ResourceType Type;

    public int MaxCount;
    public int CurrentCount;
    public int ExpectedCount;

    public ResourceParameters(ResourceType type, int currentCount, int maxCount, int expectedCount = 0)
    {
        Type = type;

        MaxCount = maxCount;
        CurrentCount = currentCount;
        ExpectedCount = expectedCount;
    }
}