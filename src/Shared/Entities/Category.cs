// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Category.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Entities;

/// <summary>
///   Represents a blog category that can be assigned to posts.
/// </summary>
[Serializable]
public class Category : Entity
{

	/// <summary>
	///   The name of the category.
	/// </summary>
	[MaxLength(80)]
	public string CategoryName { get; set; }

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public Category() : this(string.Empty) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="Category" /> class.
	/// </summary>
	/// <param name="categoryName">The categoryName of the category.</param>
	/// <param name="isArchived"></param>
	public Category(string categoryName, bool isArchived = false)
	{
		CategoryName = categoryName;
		IsArchived = isArchived;
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="Category" /> class with an explicit id. Used by tests and factories.
	/// </summary>
	/// <param name="id">The category id.</param>
	/// <param name="categoryName">The category name.</param>
	/// <param name="isArchived">Whether the category is archived.</param>
	public Category(Guid id, string categoryName, bool isArchived = false)
	{
		CategoryName = categoryName;
		IsArchived = isArchived;

		// Set the protected init Id (allowed in derived type constructor because it's protected)
		Id = id;
	}

	/// <summary>
	///   Gets an empty category instance.
	/// </summary>
	public static Category Empty => new(string.Empty) { Id = Guid.Empty };

}