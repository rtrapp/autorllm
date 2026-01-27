using AutorLLM.Domain.Interfaces;
using System.Data;

namespace AutorLLM.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation using database transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            // If no explicit transaction, just return success
            // (auto-commit mode for individual operations)
            return await Task.FromResult(1);
        }

        try
        {
            _transaction.Commit();
            return await Task.FromResult(1);
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        await Task.CompletedTask;
    }

    public void BeginTransaction()
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        _transaction = _connection.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}
