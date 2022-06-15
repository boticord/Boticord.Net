namespace Boticord.Net.Services;

internal class BucketRateLimiter
{
    private readonly int _bucketSize;

    private PeriodicTimer _emptySemaphoreTimer;

    private SemaphoreSlim _semaphore;

    public BucketRateLimiter(int bucketSize, TimeSpan bucketSpan)
    {
        _bucketSize = bucketSize;
        _semaphore = new SemaphoreSlim(0, bucketSize);
        _emptySemaphoreTimer = new PeriodicTimer(bucketSpan);
        _ = Task.Run(() => SemaphoreAutoRelease());
    }

    public Task WaitAsync()
        => _semaphore.WaitAsync();

    private async Task SemaphoreAutoRelease(CancellationToken? token = null)
    {
        while (token is null || token.Value.IsCancellationRequested)
        {
            await _emptySemaphoreTimer.WaitForNextTickAsync();
            _semaphore.Release(5);
        }
    }
}