// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Article.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlog
// Project Name :  Web
// =======================================================

using System.ComponentModel.DataAnnotations;

using MongoDB.Bson;

using Web.Data.Abstractions;
using Web.Data.Models;
using Web.Data.Validators;

namespace Web.Data.Entities;

/// <summary>
///   Represents an article in the blog system.
/// </summary>
[Serializable]
public class Article : Entity
{

	[Required]
	[MaxLength(120)]
	public string Title { get; set; }

	[MaxLength(500)]
	public string Introduction { get; set; }

	[Required]
	public string Content { get; set; }

	[MaxLength(256)]
	[Display(Name = "Cover Image URL")]
	public string CoverImageUrl { get; set; }

	[Required]
	[MaxLength(120)]
	[Display(Name = "Url Slug")]
	public string UrlSlug { get; set; }

	public AppUserDto Author { get; set; }

	public CategoryDto Category { get; set; }

	[Display(Name = "Is Published")]
	public bool IsPublished { get; set; }

	/// <summary>
	/// Indicates whether the article is archived.
	/// </summary>
	[Display(Name = "Is Archived")]
	public bool IsArchived { get; set; }

	[Display(Name = "Published On")]
	public DateTimeOffset? PublishedOn { get; set; }

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public Article() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, AppUserDto.Empty, CategoryDto.Empty, false, null, false) { }

	/// <summary>
	///   Initializes a new instance of the <see cref="Article" /> class.
	/// </summary>
	/// <param name="title">The new title</param>
	/// <param name="introduction">The new introduction</param>
	/// <param name="content">The new content</param>
	/// <param name="coverImageUrl">The new cover image URL</param>
	/// <param name="urlSlug">The new URL slug</param>
	/// <param name="author">The new author</param>
	/// <param name="category">The new category</param>
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
		AppUserDto author,
		CategoryDto category,
		bool isPublished = false,
		DateTimeOffset? publishedOn = null,
		bool isArchived = false)
	{
		Title = title;
		Introduction = introduction;
		Content = content;
		CoverImageUrl = coverImageUrl;
		UrlSlug = urlSlug;
		Author = author;
		Category = category;
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
	/// <param name="author">The new author</param>
	/// <param name="category">The new category</param>
	/// <param name="isPublished">The newly published status</param>
	/// <param name="publishedOn">The new publication date</param>
	/// /// <param name="isArchived">Indicates whether the article is archived.</param>
	/// <remarks>
	///   This method is used to update the article's properties.
	/// </remarks>
	public void Update(
				string title,
				string introduction,
				string content,
				string coverImageUrl,
				string urlSlug,
				CategoryDto category,
				bool isPublished,
				DateTimeOffset? publishedOn,
				bool isArchived)
	{
		Title = title;
		Introduction = introduction;
		Content = content;
		CoverImageUrl = coverImageUrl;
		UrlSlug = urlSlug;
		Category = category;
		IsPublished = isPublished;
		PublishedOn = publishedOn;
		IsArchived = isArchived;
	}

	public void Publish(DateTime publishedOn)
	{
		IsPublished = true;
		PublishedOn = publishedOn;
		ModifiedOn = DateTimeOffset.UtcNow;
	}

	public void Unpublish()
	{
		IsPublished = false;
		PublishedOn = null;
		ModifiedOn = DateTimeOffset.UtcNow;
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
			AppUserDto.Empty,
			CategoryDto.Empty,
			false,
			null,
			false)
	{ Id = ObjectId.Empty };

}