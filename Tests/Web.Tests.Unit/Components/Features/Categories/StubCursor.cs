// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     StubCursor.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web.Components.Features.Categories;

/// <summary>
///   Generic stub implementation of IAsyncCursor<T> for unit tests.
/// </summary>
[ExcludeFromCodeCoverage]
public class StubCursor<T> : IAsyncCursor<T>
{

	private readonly List<T> _items;

	private int _index = -1;

	public StubCursor(List<T> items) { _items = items; }

	public IEnumerable<T> Current =>
			_index >= 0 && _index < _items.Count ? new[] { _items[_index] } : Enumerable.Empty<T>();

	public bool MoveNext(CancellationToken cancellationToken = default)
	{
		return ++_index < _items.Count;
	}

	public bool MoveNext()
	{
		return MoveNext(default);
	}

	public void Dispose() { }

	public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
	{
		return Task.FromResult(MoveNext(cancellationToken));
	}

}
