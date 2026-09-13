namespace CryptoTrackingSystem.Infrastructure.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPriceRepository Prices { get; }
}
