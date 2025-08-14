// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CategoryDto.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlog
// Project Name :  Web
// =======================================================

using System.ComponentModel.DataAnnotations;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Web.Data.Validators;

namespace Web.Data.Models;

/// <summary>
///   Represents a data transfer object for a category.
/// </summary>
public class CategoryDto
{

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public CategoryDto() : this(ObjectId.Empty, string.Empty, DateTime.MinValue, null, false) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="CategoryDto" /> class.
	/// </summary>
	/// <param name="id"></param>
	/// <param name="categoryName"></param>
	/// <param name="createdOn"></param>
	/// <param name="modifiedOn"></param>
	/// <param name="archived">Indicates whether the category is archived.</param>
	private CategoryDto(
		ObjectId id,
		string categoryName,
		DateTime createdOn,
		DateTime? modifiedOn,
		bool archived = false)
	{
		Id = id;
		CategoryName = categoryName;
		CreatedOn = createdOn;
		ModifiedOn = modifiedOn;
		Archived = archived;
	}

	/// <summary>
	///   Gets or sets the unique identifier for the category.
	/// </summary>
	[BsonId]
	[BsonElement("_id")]
	[BsonRepresentation(BsonType.ObjectId)]
	public ObjectId Id { get; set; }

	/// <summary>
	///   Gets the name of the category.
	/// </summary>
	[BsonRepresentation(BsonType.String)]
	[BsonElement("categoryName")]
	[BsonRequired]
	[Display(Name = "Category Name")]
	public string CategoryName { get; set; }

	/// <summary>
	///   Gets the date and time when this entity was created.
	/// </summary>)]
	[Display(Name = "Created On")]
	[BsonElement("createdOn")]
	[BsonRequired]
	[BsonRepresentation(BsonType.DateTime)]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime CreatedOn { get; set; }

	/// <summary>
	///   Gets or sets the date and time when this entity was last modified.
	/// </summary>
	[Display(Name = "Modified On")]
	[BsonElement("modifiedOn")]
	[BsonIgnoreIfNull]
	[BsonIgnoreIfDefault]
	[BsonRepresentation(BsonType.DateTime)]
	[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
	public DateTime? ModifiedOn { get; set; }

	/// <summary>
	///   Gets or sets a value indicating whether the article is marked as deleted.
	/// </summary>
	[BsonRepresentation(BsonType.Boolean)]
	[BsonElement("archived")]
	public bool Archived { get; set; }

	/// <summary>
	///   Gets an empty category instance.
	/// </summary>
	public static CategoryDto Empty => new(ObjectId.Empty, string.Empty, DateTime.MinValue, null, false);

}