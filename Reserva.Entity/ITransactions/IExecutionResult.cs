namespace Reserva.Entity.ITransactions
{
    public interface IExecutionResult
    {
        bool IsSuccessful { get; }
    }

    public interface IExecutionResult<TResult> : IExecutionResult
    {
        TResult Result { get; }
    }
}
