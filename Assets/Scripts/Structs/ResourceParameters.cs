public struct ResourceParameters
{
    public ResourceType Type;

    public int MaxCount;
    public int CurrentCount;

    public ResourceParameters(ResourceType type, int currentCount, int maxCount)
    {
        Type = type;
        MaxCount = maxCount;
        CurrentCount = currentCount;
    }
}