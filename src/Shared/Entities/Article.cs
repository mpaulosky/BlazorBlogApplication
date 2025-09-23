// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Article.cs
// Company :       mpaulosky
// Author :        Matthew
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

	public string Title { get; set; }

	public string Introduction { get; set; }

	public string Content { get; set; }

	[Display(Name = "Cover Image URL")] public string CoverImageUrl { get; set; }

	[Display(Name = "Url Slug")] public string UrlSlug { get; set; }

	/// <summary>
	///   Foreign key to the author (AppUser)
	/// </summary>
	public string AuthorId { get; set; }

	/// <summary>
	///   Navigation property to the author (DTO expected by tests)
	/// </summary>
	public Models.AppUserDto? Author { get; set; }

	/// <summary>
	///   Foreign key to the category
	/// </summary>
	public Guid CategoryId { get; set; }

	/// <summary>
	///   Navigation property to the category (DTO expected by tests)
	/// </summary>
	public Models.CategoryDto? Category { get; set; }

	[Display(Name = "Is Published")] public bool IsPublished { get; set; }

	[Display(Name = "Published On")] public DateTime? PublishedOn { get; set; }

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public Article() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
	Guid.Empty)
	{
		// Ensure navigation DTOs are initialized to Empty to match test expectations
		Author = Models.AppUserDto.Empty;
		Category = Models.CategoryDto.Empty;
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
	/// Alternative constructor accepting Author and Category objects (used in tests)
	/// </summary>
	public Article(
			string title,
			string introduction,
			string content,
			string coverImageUrl,
			string urlSlug,
			Models.AppUserDto author,
			Models.CategoryDto category,
			bool isPublished = false,
			DateTime? publishedOn = null,
			bool isArchived = false)
	{
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
	/// Update overload accepting a CategoryDto to match tests
	/// </summary>
	public void Update(
			string title,
			string introduction,
			string content,
			string coverImageUrl,
			string urlSlug,
			CategoryDto categoryDto,
			bool isPublished,
			DateTime? publishedOn,
			bool isArchived)
	{
		Title = title;
		Introduction = introduction;
		Content = content;
		CoverImageUrl = coverImageUrl;
		UrlSlug = urlSlug;
		CategoryId = categoryDto.Id;
		Category = categoryDto;
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
	public static Article Empty { get; } = new(
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			Guid.Empty)
		{
			Id = Guid.Empty,
			AuthorId = string.Empty,
			CategoryId = Guid.Empty,
			Author = Models.AppUserDto.Empty,
			Category = Models.CategoryDto.Empty
		};

}