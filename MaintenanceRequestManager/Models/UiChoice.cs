namespace MaintenanceRequestManager.Models;

public sealed class UiChoice<T>(T value, string text)
{
    public T Value { get; } = value;
    public string Text { get; } = text;
    public override string ToString() => Text;
}
