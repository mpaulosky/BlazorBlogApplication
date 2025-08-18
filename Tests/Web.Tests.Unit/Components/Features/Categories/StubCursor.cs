using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Components.Features.Categories;

/// <summary>
///   Generic stub implementation of IAsyncCursor<T> for unit tests.
/// </summary>
public class StubCursor<T> : IAsyncCursor<T>
{
	private readonly List<T> _items;
	private int _index = -1;
	public StubCursor(List<T> items) { _items = items; }
	public IEnumerable<T> Current => _index >= 0 && _index < _items.Count ? new[] { _items[_index] } : System.Linq.Enumerable.Empty<T>();
	public bool MoveNext(CancellationToken cancellationToken = default) => ++_index < _items.Count;
	public bool MoveNext() => MoveNext(default);
	public void Dispose() { }
	public Task<bool> MoveNextAsync(CancellationToken cancellationToken) => Task.FromResult(MoveNext(cancellationToken));
}
