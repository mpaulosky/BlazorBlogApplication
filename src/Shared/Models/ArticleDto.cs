// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ArticleDto.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

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
public ArticleDto() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
ApplicationUserDto.Empty, CategoryDto.Empty, DateTime.MinValue, null, false) { }

/// <summary>
///   Initializes a new instance of the ArticleDto class.
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
Guid id,
string title,
string introduction,
string content,
string coverImageUrl,
string urlSlug,
ApplicationUserDto author,
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


public Guid Id { get; set; }

/// <summary>
///   Gets or sets the title of the article.
///   See <see cref="ArticleDtoValidator" /> for validation rules.
/// </summary>
public string Title { get; set; }

/// <summary>
///   Gets or sets the introduction or summary of the article.
///   See <see cref="ArticleDtoValidator" /> for validation rules.
/// </summary>
public string Introduction { get; set; }

/// <summary>
///   Gets or sets the main content of the article.
///   See <see cref="ArticleDtoValidator" /> for validation rules.
/// </summary>
public string Content { get; set; }

/// <summary>
///   Gets or sets the URL of the article's cover image.
///   See <see cref="ArticleDtoValidator" /> for validation rules.
/// </summary>
public string CoverImageUrl { get; set; }

/// <summary>
///   Gets or sets the URL-friendly slug for the article.
///   See <see cref="ArticleDtoValidator" /> for validation rules.
/// </summary>
public string UrlSlug { get; set; }

/// <summary>
///   Gets or sets the author information of the article.
///   See <see cref="ArticleDtoValidator" /> for validation rules.
/// </summary>
public ApplicationUserDto Author { get; set; }

/// <summary>
///   Gets or sets the category information of the article.
///   See <see cref="ArticleDtoValidator" /> for validation rules.
/// </summary>
public CategoryDto Category { get; set; }

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
///   Gets or sets a value indicating whether the article is published.
/// </summary>
[Display(Name = "Is Published")]
public bool IsPublished { get; set; }

/// <summary>
///   Gets or sets the date when the article was published.
/// </summary>
[Display(Name = "Published On")]
public DateTime? PublishedOn { get; set; }

/// <summary>
///   Gets or sets a value indicating whether the article is marked as archived.
/// </summary>
[Display(Name = "Is Archived")]
public bool IsArchived { get; set; }

/// <summary>
///   Indicates whether the current user can edit/delete this article.
/// </summary>
[Display(Name = "Can Edit")]
public bool CanEdit { get; set; }

/// <summary>
///   Gets an empty article instance.
/// </summary>
public static ArticleDto Empty { get; } =
new(
Guid.Empty,
string.Empty,
string.Empty,
string.Empty,
string.Empty,
string.Empty,
ApplicationUserDto.Empty,
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

// Convert entity navigation properties to DTOs
Author = article.Author != null 
? new ApplicationUserDto
{
Id = article.Author.Id,
UserName = article.Author.UserName ?? string.Empty,
Email = article.Author.Email ?? string.Empty,
DisplayName = article.Author.DisplayName,
EmailConfirmed = article.Author.EmailConfirmed
}
: ApplicationUserDto.Empty,
Category = article.Category != null 
? CategoryDto.FromEntity(article.Category) 
: CategoryDto.Empty,
CreatedOn = article.CreatedOn,
ModifiedOn = article.ModifiedOn,
IsPublished = article.IsPublished,
PublishedOn = article.PublishedOn,
IsArchived = article.IsArchived,
CanEdit = false
};
}

}
