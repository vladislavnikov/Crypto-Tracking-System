namespace CryptoTrackingSystem.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPriceRepository Prices { get; }
}
