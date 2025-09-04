// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleDto.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

using Shared.Validators;

namespace Shared.Models;

/// <summary>
///   Data Transfer Object (DTO) representing an article.
///   All validations are handled by <see cref="ArticleDtoValidator" />.
/// </summary>
public sealed class ArticleDto
{

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public ArticleDto() : this(ObjectId.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, AppUserDto.Empty, CategoryDto.Empty, DateTime.MinValue, null, false) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="ArticleDto" /> class.
	/// </summary>
	/// <param name="id"></param>
	/// <param name="title">The new title</param>
	/// <param name="introduction">The new introduction</param>
	/// <param name="content">The new content</param>
	/// <param name="coverImageUrl">The new cover image URL</param>
	/// <param name="urlSlug">The new URL slug</param>
	/// <param name="author">The new author</param>
	/// <param name="category">The new category</param>
	/// <param name="createdOn">The date the item was created</param>
	/// <param name="modifiedOn">The date the item was modified</param>
	/// <param name="isPublished">The newly published status</param>
	/// <param name="publishedOn">The new publication date</param>
	/// <param name="isArchived">Gets or sets the archived status of the entity.</param>
	/// <param name="canEdit"></param>
	private ArticleDto(
		ObjectId id,
		string title,
		string introduction,
		string content,
		string coverImageUrl,
		string urlSlug,
		AppUserDto author,
		CategoryDto category,
		DateTime createdOn,
		DateTime? modifiedOn,
		bool isPublished,
		DateTime? publishedOn = null,
		bool isArchived = false,
		bool canEdit = false)
	{
		Id = id;
		Title = title;
		Introduction = introduction;
		Content = content;
		CoverImageUrl = coverImageUrl;
		UrlSlug = urlSlug;
		Author = author;
		Category = category;
		CreatedOn = createdOn;
		ModifiedOn = modifiedOn;
		IsPublished = isPublished;
		PublishedOn = publishedOn;
		IsArchived = isArchived;
		CanEdit = canEdit;
	}

	/// <summary>
	///   Gets or sets the unique identifier for the article.
	/// </summary>
	[BsonId]
	[BsonElement("_id")]
	[BsonRepresentation(BsonType.ObjectId)]
	public ObjectId Id { get; set; }

	/// <summary>
	///   Gets or sets the title of the article.
	///   See <see cref="ArticleDtoValidator" /> for validation rules.
	/// </summary>
	[BsonElement("title")]
	[BsonRequired]
	[BsonRepresentation(BsonType.String)]
	public string Title { get; set; }

	/// <summary>
	///   Gets or sets the introduction or summary of the article.
	///   See <see cref="ArticleDtoValidator" /> for validation rules.
	/// </summary>
	[BsonElement("introduction")]
	[BsonRequired]
	[BsonRepresentation(BsonType.String)]
	public string Introduction { get; set; }

	/// <summary>
	///   Gets or sets the main content of the article.
	///   See <see cref="ArticleDtoValidator" /> for validation rules.
	/// </summary>
	[BsonElement("content")]
	[BsonRequired]
	[BsonRepresentation(BsonType.String)]
	public string Content { get; set; }

	/// <summary>
	///   Gets or sets the URL of the article's cover image.
	///   See <see cref="ArticleDtoValidator" /> for validation rules.
	/// </summary>
	[BsonElement("coverImageUrl")]
	[BsonRequired]
	[BsonRepresentation(BsonType.String)]
	public string CoverImageUrl { get; set; }

	/// <summary>
	///   Gets or sets the URL-friendly slug for the article.
	///   See <see cref="ArticleDtoValidator" /> for validation rules.
	/// </summary>
	[BsonElement("urlSlug")]
	[BsonRequired]
	[BsonRepresentation(BsonType.String)]
	public string UrlSlug { get; set; }

	/// <summary>
	///   Gets or sets the author information of the article.
	///   See <see cref="ArticleDtoValidator" /> for validation rules.
	/// </summary>
	[BsonElement("author")]
	[BsonRequired]
	public AppUserDto Author { get; set; }

	/// <summary>
	///   Gets or sets the category information of the article.
	///   See <see cref="ArticleDtoValidator" /> for validation rules.
	/// </summary>
	[BsonElement("category")]
	[BsonRequired]
	public CategoryDto Category { get; set; }

	/// <summary>
	///   Gets the date and time when this entity was created.
	/// </summary>)]
	[Display(Name = "Created On")]
	[BsonElement("createdOn")]
	[BsonRequired]
	[BsonRepresentation(BsonType.DateTime)]
	public DateTime CreatedOn { get; set; }

	/// <summary>
	///   Gets or sets the date and time when this entity was last modified.
	/// </summary>
	[Display(Name = "Modified On")]
	[BsonElement("modifiedOn")]
	[BsonIgnoreIfNull]
	[BsonIgnoreIfDefault]
	[BsonRepresentation(BsonType.DateTime)]
	public DateTime? ModifiedOn { get; set; }

	/// <summary>
	///   Gets or sets a value indicating whether the article is published.
	/// </summary>
	[Display(Name = "Is Published")]
	[BsonElement("isPublished")]
	[BsonRepresentation(BsonType.Boolean)]
	public bool IsPublished { get; set; }

	/// <summary>
	///   Gets or sets the date when the article was published.
	/// </summary>
	[Display(Name = "Published On")]
	[BsonElement("publishedOn")]
	[BsonIgnoreIfNull]
	[BsonIgnoreIfDefault]
	[BsonRepresentation(BsonType.DateTime)]
	public DateTime? PublishedOn { get; set; }

	/// <summary>
	///   Gets or sets a value indicating whether the article is marked as archived.
	/// </summary>
	[BsonRepresentation(BsonType.Boolean)]
	[BsonElement("isArchived")]
	public bool IsArchived { get; set; }

	/// <summary>
	///   Indicates whether the current user can edit/delete this article.
	/// </summary>
	[BsonRepresentation(BsonType.Boolean)]
	[BsonElement("canEdit")]
	public bool CanEdit { get; set; }

	/// <summary>
	///   Gets an empty article instance.
	/// </summary>
	public static ArticleDto Empty { get; } =
		new(
			ObjectId.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			AppUserDto.Empty,
			CategoryDto.Empty,
			DateTime.MinValue,
			null,
			false
		);

	public static ArticleDto FromEntity(Article article)
	{
		return new ArticleDto
		{
			Id = article.Id,
			Title = article.Title,
			Introduction = article.Introduction,
			Content = article.Content,
			CoverImageUrl = article.CoverImageUrl,
			UrlSlug = article.UrlSlug,
			Author = article.Author,
			Category = article.Category,
			CreatedOn = article.CreatedOn,
			ModifiedOn = article.ModifiedOn,
			IsPublished = article.IsPublished,
			PublishedOn = article.PublishedOn,
			IsArchived = article.IsArchived,
			CanEdit = false
		};
	}

}
