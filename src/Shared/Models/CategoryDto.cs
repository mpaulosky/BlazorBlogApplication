// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryDto.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Models;

/// <summary>
///   Represents a data transfer object for a category.
/// </summary>
public class CategoryDto
{

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public CategoryDto() : this(Guid.Empty, string.Empty, DateTime.UtcNow, null, false) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="CategoryDto" /> class.
	/// </summary>
	/// <param name="id"></param>
	/// <param name="categoryName"></param>
	/// <param name="createdOn"></param>
	/// <param name="modifiedOn"></param>
	/// <param name="isArchived">Indicates whether the category is archived.</param>
	private CategoryDto(
			Guid id,
			string categoryName,
			DateTime createdOn,
			DateTime? modifiedOn,
			bool isArchived)
	{
		Id = id;
		CategoryName = categoryName;
		CreatedOn = createdOn;
		ModifiedOn = modifiedOn;
		IsArchived = isArchived;
	}

	/// <summary>
	///   Gets or sets the unique identifier for the category.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	///   Gets the name of the category.
	/// </summary>
	[Display(Name = "Category Name")]
	public string CategoryName { get; set; }

	/// <summary>
	///   Gets the date and time when this entity was created.
	/// </summary>
	/// )]
	[Display(Name = "Created On")]
	public DateTime CreatedOn { get; set; }

	/// <summary>
	///   Gets or sets the date and time when this entity was last modified.
	/// </summary>
	[Display(Name = "Modified On")]
	public DateTime? ModifiedOn { get; set; }

	/// <summary>
	///   Gets or sets a value indicating whether the category is archived.
	/// </summary>
	[Display(Name = "Is Archived")]
	public bool IsArchived { get; set; }

	/// <summary>
	///   Backwards-compatible alias for IsArchived used throughout UI code.
	///   Some code expects a property named 'Archived' on DTOs; expose it as an alias.
	/// </summary>
	public bool Archived
	{
		get => IsArchived;

		set => IsArchived = value;
	}

	/// <summary>
	///   Gets an empty singleton category instance.
	/// </summary>
	public static CategoryDto Empty { get; } = new(Guid.Empty, string.Empty, DateTime.UtcNow, null, false);

	public static CategoryDto FromEntity(Category c)
	{
		return new CategoryDto
		{
				Id = c.Id,
				CategoryName = c.CategoryName,
				CreatedOn = c.CreatedOn,
				ModifiedOn = c.ModifiedOn,
				IsArchived = c.IsArchived
		};
	}

}