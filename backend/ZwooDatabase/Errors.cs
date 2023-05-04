namespace ZwooDatabase;

public interface ErrorAble<T>
{
    public T Value { get; }
    public ErrorCode? Error { get; }
}

public enum ErrorCode { }