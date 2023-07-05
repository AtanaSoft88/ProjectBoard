namespace ProjectBoard.API.Configuration;

public class FeatureConfig
{
    public FeatureConfig()
    {
        Operations = new List<OperationConfig>();
        Name = string.Empty;
        NameSpace = string.Empty;
    }
    public string Name { get; set; }
    public string NameSpace { get; set; }
    public List<OperationConfig> Operations { get; set; }


    public static List<OperationConfig> DefaultOperations =>
        new List<OperationConfig>()
        {
            new OperationConfig {
                    OperationName = Constants.GetOperation,
                    Policy = Constants.UserPolicyName
            },
            new OperationConfig {
                    OperationName = Constants.GetAllOperation,
                    Policy = Constants.UserPolicyName
            },
            new OperationConfig {
                    OperationName = Constants.UpdateOperation,
                    Policy = Constants.UserPolicyName
            },
            new OperationConfig {
                    OperationName = Constants.CreateOperation,
                    Policy = Constants.UserPolicyName
            }
        };
    

    public void MergeDefaults()
    {
        foreach (var defaultOperation in DefaultOperations)
        {
            if (Operations.FirstOrDefault(o => o.OperationName == defaultOperation.OperationName) == null)
            {
                Operations.Add(defaultOperation);
            }
        }
    }
}
