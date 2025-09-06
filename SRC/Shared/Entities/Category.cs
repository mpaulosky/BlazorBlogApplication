// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Category.cs
// Company :       mpaulosky
// Author :        Matthew
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
	///   Indicates whether the article is archived.
	/// </summary>
	[Display(Name = "Is Archived")]
	public bool IsArchived { get; set; }

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public Category() : this(string.Empty, false) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="Category" /> class.
	/// </summary>
	/// <param name="categoryName">The categoryName of the category.</param>
	public Category(string categoryName, bool isArchived = false)
	{
		CategoryName = categoryName;
		IsArchived = isArchived;
	}

	/// <summary>
	///   Gets an empty category instance.
	/// </summary>
	public static Category Empty =>
			new(string.Empty, false)
			{
					Id = ObjectId.Empty,
					IsArchived = false
			};

}
