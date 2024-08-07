namespace EatConscious.Wrappers;

public interface IWrapper<T>
{
    public static abstract T StateOnLoad { get; }
}