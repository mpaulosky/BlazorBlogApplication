// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Article.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================
namespace Shared.Entities;

/// <summary>
///   Represents an article in the blog system.
/// </summary>
[Serializable]
public class Article : Entity
{

	private static void EnsureRequired(string value, string paramName)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException($"{paramName} is required and cannot be empty.", paramName);
		}
	}


	public string Title { get; set; }

	public string Introduction { get; set; }

	public string Content { get; set; }

	[Display(Name = "Cover Image URL")] public string CoverImageUrl { get; set; }

	[Display(Name = "Url Slug")] public string UrlSlug { get; set; }

	/// <summary>
	///   Foreign key to the author (ApplicationUser)
	/// </summary>
	public string AuthorId { get; set; }

	/// <summary>
	///   Navigation property to the author (DTO expected by tests)
	/// </summary>
	public ApplicationUserDto Author { get; set; } = ApplicationUserDto.Empty;

	/// <summary>
	///   Foreign key to the category
	/// </summary>
	public Guid CategoryId { get; set; }

	/// <summary>
	///   Navigation property to the category (DTO expected by tests)
	/// </summary>
	public CategoryDto Category { get; set; } = CategoryDto.Empty;

	[Display(Name = "Is Published")] public bool IsPublished { get; set; }

	[Display(Name = "Published On")] public DateTime? PublishedOn { get; set; }


	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	///   This bypasses validation and creates an empty/default article used by tests and serializers.
	/// </summary>
	public Article()
	{
		Title = string.Empty;
		Introduction = string.Empty;
		Content = string.Empty;
		CoverImageUrl = string.Empty;
		UrlSlug = string.Empty;
		AuthorId = string.Empty;
		CategoryId = Guid.Empty;
		IsPublished = false;
		PublishedOn = null;
		IsArchived = false;

		// Ensure navigation DTOs are initialized to Empty to match test expectations
		Author = ApplicationUserDto.Empty;
		Category = CategoryDto.Empty;
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="Article" /> class.
	/// </summary>
	/// <param name="title">The new title</param>
	/// <param name="introduction">The new introduction</param>
	/// <param name="content">The new content</param>
	/// <param name="coverImageUrl">The new cover image URL</param>
	/// <param name="urlSlug">The new URL slug</param>
	/// <param name="authorId">The author's ID</param>
	/// <param name="categoryId">The category's ID</param>
	/// <param name="isPublished">The newly published status</param>
	/// <param name="publishedOn">The new publication date</param>
	/// <param name="isArchived">Indicates whether the article is archived.</param>
	/// <remarks>
	///   This constructor is used to create a new article instance with all required properties.
	/// </remarks>
	public Article(
			string title,
			string introduction,
			string content,
			string coverImageUrl,
			string urlSlug,
			string authorId,
			Guid categoryId,
			bool isPublished = false,
			DateTime? publishedOn = null,
			bool isArchived = false)
	{
		EnsureRequired(title, nameof(title));
		EnsureRequired(content, nameof(content));
		EnsureRequired(urlSlug, nameof(urlSlug));
		Title = title;
		Introduction = introduction;
		Content = content;
		CoverImageUrl = coverImageUrl;
		UrlSlug = urlSlug;
		AuthorId = authorId;
		CategoryId = categoryId;
		IsPublished = isPublished;
		PublishedOn = publishedOn;
		IsArchived = isArchived;
	}

	/// <summary>
	///   Alternative constructor accepting Author and Category objects (used in tests)
	/// </summary>
	/// <remarks>
	///   Added overloads will accept the newer <c>ApplicationUserDto</c> while
	///   mapping to the legacy <c>ApplicationUserDto</c> to remain backward compatible.
	/// </remarks>
	public Article(
			string title,
			string introduction,
			string content,
			string coverImageUrl,
			string urlSlug,
			ApplicationUserDto author,
			CategoryDto category,
			bool isPublished = false,
			DateTime? publishedOn = null,
			bool isArchived = false)
	{
		EnsureRequired(title, nameof(title));
		EnsureRequired(content, nameof(content));
		EnsureRequired(urlSlug, nameof(urlSlug));
		Title = title;
		Introduction = introduction;
		Content = content;
		CoverImageUrl = coverImageUrl;
		UrlSlug = urlSlug;
		Author = author;
		AuthorId = string.Empty;
		Category = category;
		CategoryId = Guid.Empty;
		IsPublished = isPublished;
		PublishedOn = publishedOn;
		IsArchived = isArchived;
	}

	/// <summary>
	///   Updates the article's properties
	/// </summary>
	/// <param name="title">The new title</param>
	/// <param name="introduction">The new introduction</param>
	/// <param name="content"></param>
	/// <param name="coverImageUrl">The new cover image URL</param>
	/// <param name="urlSlug">The new URL slug</param>
	/// <param name="categoryId">The new category ID</param>
	/// <param name="isPublished">The newly published status</param>
	/// <param name="publishedOn">The new publication date</param>
	/// <param name="isArchived">Indicates whether the article is archived.</param>
	/// <remarks>
	///   This method is used to update the article's properties.
	/// </remarks>
	public void Update(
			string title,
			string introduction,
			string content,
			string coverImageUrl,
			string urlSlug,
			Guid categoryId,
			bool isPublished,
			DateTime? publishedOn,
			bool isArchived)
	{
		EnsureRequired(title, nameof(title));
		EnsureRequired(content, nameof(content));
		EnsureRequired(urlSlug, nameof(urlSlug));
		Title = title;
		Introduction = introduction;
		Content = content;
		CoverImageUrl = coverImageUrl;
		UrlSlug = urlSlug;
		CategoryId = categoryId;
		IsPublished = isPublished;
		PublishedOn = publishedOn;
		IsArchived = isArchived;
	}

	public void Publish(DateTime publishedOn)
	{
		IsPublished = true;
		PublishedOn = publishedOn;
		ModifiedOn = DateTime.UtcNow;
	}

	public void Unpublish()
	{
		IsPublished = false;
		PublishedOn = null;
		ModifiedOn = DateTime.UtcNow;
	}

	/// <summary>
	///   Gets an empty article instance.
	/// </summary>
	public static Article Empty { get; } = new()
	{
			Id = Guid.Empty,
			AuthorId = string.Empty,
			CategoryId = Guid.Empty,
			Author = ApplicationUserDto.Empty,
			Category = CategoryDto.Empty
	};

}
