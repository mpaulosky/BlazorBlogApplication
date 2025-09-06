// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     StubCursor.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

namespace Web;

/// <summary>
///   Generic stub implementation of IAsyncCursor for unit tests.
/// </summary>
[ExcludeFromCodeCoverage]
public class StubCursor<T> : IAsyncCursor<T>
{

	private readonly List<T> _items;

	private int _index = -1;

	public StubCursor(List<T> items) { _items = items; }

	public IEnumerable<T> Current => _index >= 0 && _index < _items.Count ? [ _items[_index] ] : [];

	public bool MoveNext(CancellationToken cancellationToken = default)
	{
		return ++_index < _items.Count;
	}

	public void Dispose() { }

	public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
	{
		return Task.FromResult(MoveNext(cancellationToken));
	}

}
