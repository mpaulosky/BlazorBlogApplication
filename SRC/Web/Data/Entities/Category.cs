// =======================================================
// Copyright (c) 2025. All rights reserved.
// File CategoryName :     Categories.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution CategoryName : MyBlog
// Project CategoryName :  Web
// =======================================================

using System.ComponentModel.DataAnnotations;

using MongoDB.Bson;

using Web.Data.Abstractions;

namespace Web.Data.Entities;

/// <summary>
///   Represents a blog category that can be assigned to posts.
/// </summary>
[Serializable]
public class Category : Entity
{

	[Required(ErrorMessage = "CategoryName is required")]
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
	public Category(string categoryName)
	{
		CategoryName = categoryName;
	}

	/// <summary>
	///   Gets an empty category instance.
	/// </summary>
	public static Category Empty => new(string.Empty)
	{
		Id = ObjectId.Empty
	};

}