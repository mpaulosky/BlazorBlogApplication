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
using Web.Data.Validators;

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
	public Category() : this(string.Empty, true) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="Category" /> class.
	/// </summary>
	/// <param name="categoryName">The categoryName of the category.</param>
	/// <param name="skipValidation">If true, skips validation on construction.</param>
	/// <exception cref="ValidationException">Thrown when validation fails</exception>
	public Category(string categoryName, bool skipValidation = false)
	{
		CategoryName = categoryName;

		if (!skipValidation)
		{
			ValidateState();
		}
	}

	/// <summary>
	///   Gets an empty category instance.
	/// </summary>
	public static Category Empty => new(string.Empty, true)
	{
		Id = ObjectId.Empty
	};

	/// <summary>
	///   Validates the current state of the category.
	/// </summary>
	/// <exception cref="ValidationException">Thrown when validation fails.</exception>
	private void ValidateState()
	{
		var validator = new CategoryValidator();
		var validationResult = validator.Validate(this);

		if (!validationResult.IsValid)
		{
			throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
		}
	}

}