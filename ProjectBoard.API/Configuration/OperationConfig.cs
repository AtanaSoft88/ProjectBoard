namespace ProjectBoard.API.Configuration;

public class OperationConfig : IEquatable<OperationConfig>
{
    public string OperationName { get; set; }
    public string Policy { get; set; }

    public bool Equals(OperationConfig? other)
    {
        return other != null && other.OperationName == OperationName;
    }
}
