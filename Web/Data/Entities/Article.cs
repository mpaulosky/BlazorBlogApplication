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

	[Display(Name = "Published On")]
	public DateTimeOffset? PublishedOn { get; set; }

	/// <summary>
	///   Parameterless constructor for serialization and test data generation.
	/// </summary>
	public Article() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, AppUserDto.Empty, CategoryDto.Empty, false, null, true) { }

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
	/// <param name="skipValidation">If true, skips validation on construction.</param>
	/// <exception cref="ValidationException">Thrown when validation fails</exception>
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
			bool skipValidation = false)
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

		if (!skipValidation)
		{
			ValidateState();
		}
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
	/// <exception cref="ValidationException">Thrown when validation fails</exception>
	public void Update(
			string title,
			string introduction,
			string content,
			string coverImageUrl,
			string urlSlug,
			AppUserDto author,
			CategoryDto category,
			bool isPublished,
			DateTimeOffset? publishedOn)
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
		ValidateState();
	}

	public void Publish(DateTime publishedOn)
	{
		IsPublished = true;
		PublishedOn = publishedOn;
		ModifiedOn = DateTimeOffset.UtcNow;
		ValidateState();
	}

	public void Unpublish()
	{
		IsPublished = false;
		PublishedOn = null;
		ModifiedOn = DateTimeOffset.UtcNow;
		ValidateState();
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
			CategoryDto.Empty)

	{
		Id = ObjectId.Empty
	};

	private void ValidateState()
	{
		var validator = new ArticleValidator();
		var validationResult = validator.Validate(this);

		if (!validationResult.IsValid)
		{
			throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
		}
	}

}