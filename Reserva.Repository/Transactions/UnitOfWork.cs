using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
//using Reserva.Audit.RestClient;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Base;
using Reserva.Repository.Extensions;
using Storage = Microsoft.EntityFrameworkCore.Storage;
using System;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
namespace Reserva.Repository.Transactions
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private DbContext Context => _dbContext;

        //protected readonly IAuditService _auditService;
        private readonly ILogger<UnitOfWork<TContext>> _logger;

        private readonly TimezoneInfoData _timezoneInfoData;

        public UnitOfWork(TContext dbContext, /*IAuditService auditService, */ILogger<UnitOfWork<TContext>> logger, TimezoneInfoData timezoneInfoData)
        {
            _dbContext = dbContext;
            //_auditService = auditService;
            _logger = logger;
            _timezoneInfoData = timezoneInfoData;
        }

        public ITransaction BeginTransaction()
            => new Transaction(Context.Database.BeginTransaction());

        public ITransaction? GetCurrentTransaction()
            => Context.Database.CurrentTransaction != null ? new Transaction(Context.Database.CurrentTransaction) : null;

        public void Execute<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded, Action onComplete)
            => ExecuteOperation(state, operation, verifySucceeded, onComplete!);

        public void ExecuteInTransaction<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded, Action<bool> onComplete)
        {
            ExecuteOperation(
                state,
                stateIn => ExecuteInTransaction(
                    stateIn,
                    stateTransaction => { operation.Invoke(stateTransaction); return string.Empty; },
                    (transactionInCourse, _) => onComplete?.Invoke(transactionInCourse)
                ),
                verifySucceeded,
                null!
            );
        }

        private void ExecuteOperation<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded, Action onComplete)
        {
            ExecuteStrategy(
                state,
                stateIn => { operation.Invoke(stateIn); return default; },
                stateIn =>
                {
                    return verifySucceeded != null ?
                        new Storage.ExecutionResult<TState>(verifySucceeded.Invoke(stateIn).IsSuccessful, default) :
                        new Storage.ExecutionResult<TState>(true, default);
                },
                (_) => onComplete?.Invoke()
            );
        }

        public async Task ExecuteAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, Func<Task> onComplete, CancellationToken cancellationToken = default)
            => await ExecuteOperationAsync(state, operation, verifySucceeded, onComplete!, cancellationToken);

        public async Task ExecuteInTransactionAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, Func<bool, Task> onComplete, CancellationToken cancellationToken = default)
        {
            await ExecuteOperationAsync(
                state,
                async (stateIn, cancellationTokenIn) => await ExecuteInTransactionAsync(
                    stateIn,
                    async (stateTransaction, cancellationTokenTransaction) => { await operation.Invoke(stateTransaction, cancellationTokenTransaction); return string.Empty; },
                    async (transactionInCourse, _) => await onComplete.Invoke(transactionInCourse),
                    cancellationTokenIn
                ),
                verifySucceeded,
                null!,
                cancellationToken
            );
        }

        private async Task ExecuteOperationAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, Func<Task> onComplete, CancellationToken cancellationToken = default)
        {
            await ExecuteStrategyAsync(
                state,
                async (stateIn, cancellationTokenIn) => { await operation.Invoke(stateIn, cancellationTokenIn); return default; },
                async (stateIn, cancellationTokenIn) =>
                {
                    return verifySucceeded != null ?
                        new Storage.ExecutionResult<TState>((await verifySucceeded.Invoke(stateIn, cancellationTokenIn)).IsSuccessful, default) :
                        new Storage.ExecutionResult<TState>(true, default);
                },
                async (_) => await onComplete?.Invoke()!,
                cancellationToken
            );
        }

        public TResult Execute<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded, Action<TResult> onComplete)
            => ExecuteOperation(state, operation, verifySucceeded, onComplete!);

        public TResult ExecuteInTransaction<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded, Action<bool, TResult> onComplete)
        {
            return ExecuteOperation(
                state,
                stateIn => ExecuteInTransaction(stateIn, operation, onComplete!),
                verifySucceeded,
                null!
            );
        }

        private TResult ExecuteOperation<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded, Action<TResult> onComplete)
        {
            return ExecuteStrategy(
                state,
                stateIn => operation.Invoke(stateIn),
                stateIn =>
                {
                    if (verifySucceeded != null)
                    {
                        var result = verifySucceeded.Invoke(stateIn);
                        return new Storage.ExecutionResult<TResult>(result.IsSuccessful, result.Result);
                    }

                    return new Storage.ExecutionResult<TResult>(true, default);
                },
                onComplete!
            );
        }

        public async Task<TResult> ExecuteAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, Func<TResult, Task> onComplete, CancellationToken cancellationToken = default)
            => await ExecuteOperationAsync(state, operation, verifySucceeded, onComplete!, cancellationToken);

        public async Task<TResult> ExecuteInTransactionAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, Func<bool, TResult, Task> onComplete, CancellationToken cancellationToken = default)
        {
            return await ExecuteOperationAsync(
                state,
                async (stateIn, cancellationTokenIn) => await ExecuteInTransactionAsync(stateIn, operation, onComplete!, cancellationTokenIn),
                verifySucceeded,
                null!,
                cancellationToken
            );
        }

        private async Task<TResult> ExecuteOperationAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, Func<TResult, Task> onComplete, CancellationToken cancellationToken = default)
        {
            return await ExecuteStrategyAsync(
                state,
                async (stateIn, cancellationTokenIn) => await operation.Invoke(stateIn, cancellationTokenIn),
                async (stateIn, cancellationTokenIn) =>
                {
                    if (verifySucceeded != null)
                    {
                        var result = await verifySucceeded.Invoke(stateIn, cancellationTokenIn);
                        return new Storage.ExecutionResult<TResult>(result.IsSuccessful, result.Result);
                    }

                    return new Storage.ExecutionResult<TResult>(true, default);
                },
                onComplete!,
                cancellationToken
            );
        }

        private TResult ExecuteStrategy<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, Storage.ExecutionResult<TResult>> verifySucceeded, Action<TResult> onComplete)
        {
            var executionStrategy = Context.Database.CreateExecutionStrategy();

            return executionStrategy.Execute(
                state,
                (context, stateIn) =>
                {
                    try
                    {
                        var result = operation.Invoke(stateIn);
                        Commit();
                        onComplete?.Invoke(result);
                        return result;
                    }
                    catch (Exception)
                    {
                        ClearChangeTracker();
                        throw;
                    }
                },
                (context, stateIn) => verifySucceeded.Invoke(stateIn)
            );
        }

        private async Task<TResult> ExecuteStrategyAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<Storage.ExecutionResult<TResult>>> verifySucceeded, Func<TResult, Task> onComplete, CancellationToken cancellationToken = default)
        {
            var executionStrategy = Context.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(
                state,
                async (context, stateIn, cancellationTokenIn) =>
                {
                    try
                    {
                        var result = await operation.Invoke(stateIn, cancellationTokenIn);
                        await CommitAsync();
                        if (onComplete != null) await onComplete.Invoke(result);
                        return result;
                    }
                    catch (Exception)
                    {
                        ClearChangeTracker();
                        throw;
                    }
                },
                async (context, stateIn, cancellationTokenIn) => await verifySucceeded.Invoke(stateIn, cancellationTokenIn),
                cancellationToken
            );
        }

        private TResult ExecuteInTransaction<TState, TResult>(TState state, Func<TState, TResult> operation, Action<bool, TResult> onComplete)
        {
            var transactionInCourse = GetCurrentTransaction()?.TransactionId != default;

            if (transactionInCourse)
                return operation.Invoke(state);

            using var transaction = BeginTransaction();

            try
            {
                var result = operation.Invoke(state);
                transaction.Commit();
                onComplete?.Invoke(transactionInCourse, result);
                return result;
            }
            catch (ResultException<TResult> rex)
            {
                transaction.Rollback();
                ClearChangeTracker();
                return rex.Result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                ClearChangeTracker();
                throw;
            }
        }

        private async Task<TResult> ExecuteInTransactionAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<bool, TResult, Task> onComplete, CancellationToken cancellationToken = default)
        {
            var transactionInCourse = GetCurrentTransaction()?.TransactionId != default;

            if (transactionInCourse)
                return await operation.Invoke(state, cancellationToken);

            using var transaction = BeginTransaction();

            try
            {
                var result = await operation.Invoke(state, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                if (onComplete != null) await onComplete.Invoke(transactionInCourse, result);
                return result;
            }
            catch (ResultException<TResult> rex)
            {
                transaction.Rollback();
                ClearChangeTracker();
                return rex.Result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                ClearChangeTracker();
                throw;
            }
        }

        public void Commit()
        {
            UpdateOffsetTimezone();
            _dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            UpdateOffsetTimezone();
            await _dbContext.SaveChangesAsync();
        }

        private void UpdateOffsetTimezone()
        {
            var timezone = _timezoneInfoData.TimeZone;
            Console.WriteLine("Display TimeZone: " + timezone.DisplayName);
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                foreach (var propertyInfo in entry.Entity.GetType().GetTypeProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)))
                {
                    var dateTimeOffset = entry.CurrentValues.GetValue<DateTimeOffset>(propertyInfo.Name);
                    var newOffset = timezone.GetUtcOffset(dateTimeOffset.UtcDateTime);
                    try
                    {
                        var newDateTimeOffset = dateTimeOffset.ToOffset(newOffset);

                        // Validar que el nuevo DateTimeOffset esté dentro de los límites permitidos
                        if (newDateTimeOffset >= DateTimeOffset.MinValue && newDateTimeOffset <= DateTimeOffset.MaxValue)
                        {
                            propertyInfo.SetValue(entry.Entity, newDateTimeOffset);
                        }
                        else
                        {
                            Console.WriteLine($"Original: {dateTimeOffset}, Nuevo: {newDateTimeOffset}, Offset: {newOffset}");
                            // Manejar el caso donde la nueva fecha estaría fuera de rango
                            Console.WriteLine($"El valor ajustado de {propertyInfo.Name} está fuera del rango permitido.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine($"{propertyInfo.Name} Original: {dateTimeOffset}, Offset: {newOffset}");
                    }

                }

                foreach (var propertyInfo in entry.Entity.GetType().GetTypeProperties().Where(p => p.PropertyType == typeof(DateTimeOffset?)))
                {
                    var dateTimeOffset = entry.CurrentValues.GetValue<DateTimeOffset?>(propertyInfo.Name);
                    try
                    {
                        if (dateTimeOffset.HasValue)
                        {
                            var newOffset = timezone.GetUtcOffset(dateTimeOffset.Value.UtcDateTime);

                            Console.WriteLine($"{propertyInfo.Name} Original: {dateTimeOffset}, Offset: {newOffset}");

                            var newDateTimeOffset = dateTimeOffset.Value.ToOffset(newOffset);

                            // Validar que el nuevo DateTimeOffset esté dentro de los límites permitidos
                            if (newDateTimeOffset >= DateTimeOffset.MinValue && newDateTimeOffset <= DateTimeOffset.MaxValue)
                            {
                                propertyInfo.SetValue(entry.Entity, newDateTimeOffset);
                            }
                            else
                            {
                                Console.WriteLine($"Original: {dateTimeOffset}, Nuevo: {newDateTimeOffset}, Offset: {newOffset}");
                                Console.WriteLine($"El valor ajustado de {propertyInfo.Name} está fuera del rango permitido.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        var currentOffset = timezone.GetUtcOffset(DateTime.UtcNow);
                        Console.WriteLine($"{propertyInfo.Name} Original: {dateTimeOffset}, Current Offset: {currentOffset}");
                    }

                }
            }
        }

        public void SendAudit(bool openThread = true, bool verbose = false)
        {
            //if (openThread)
            //{
            //    new Thread(() =>
            //    {
            //        try { SendAuditData(verbose); }
            //        catch (Exception ex) { _logger.LogError(ex, ex.Message); }
            //    }).Start();
            //}
            //else
            //{
            //    try { SendAuditData(verbose); }
            //    catch (Exception ex) { _logger.LogError(ex, ex.Message); }
            //}
        }

        private void SendAuditData(bool verbose)
        {
            if (verbose) _logger.LogInformation("Start auditing data...");

            Task task = new(() =>
            {
                //await _auditService.SaveChanges();
                //_auditService.ClearChanges();
            });
            task.RunSynchronously();

            if (verbose) _logger.LogInformation("End auditing data...");
        }

        private void ClearChangeTracker()
        {
            Context.ChangeTracker.Clear();
        }
    }
}
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8604 // Possible null reference argument.
