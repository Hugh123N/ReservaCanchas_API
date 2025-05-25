namespace Reserva.Repository.Abstractions.Transactions
{
    public interface IUnitOfWork
    {
        ITransaction BeginTransaction();
        ITransaction? GetCurrentTransaction();
        void Execute<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded, Action onComplete);
        void ExecuteInTransaction<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded, Action<bool> onComplete);
        Task ExecuteAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, Func<Task> onComplete, CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, Func<bool, Task> onComplete, CancellationToken cancellationToken = default);
        TResult Execute<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded, Action<TResult> onComplete);
        TResult ExecuteInTransaction<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded, Action<bool, TResult> onComplete);
        Task<TResult> ExecuteAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, Func<TResult, Task> onComplete, CancellationToken cancellationToken = default);
        Task<TResult> ExecuteInTransactionAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, Func<bool, TResult, Task> onComplete, CancellationToken cancellationToken = default);
        void Commit();
        Task CommitAsync();
        void SendAudit(bool openThread = true, bool verbose = false);
    }
}
