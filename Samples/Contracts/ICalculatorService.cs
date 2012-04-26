namespace Contracts
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "urn:Samples:Discovery:1.0")]
    public interface ICalculatorService
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

    public interface ICalculatorServiceClient : IClientChannel, ICalculatorService
    {
    }
}
