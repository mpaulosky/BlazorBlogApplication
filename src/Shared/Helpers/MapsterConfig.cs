// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MapsterConfig.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

#region

using Mapster;

#endregion

namespace Shared.Helpers;

/// <summary>
///   Configures Mapster mappings for the Web project.
/// </summary>
public static class MapsterConfig
{

	public static void RegisterMappings()
	{
		TypeAdapterConfig<Article, ArticleDto>.NewConfig()
				.Map(dest => dest.Id, src => src.Id)
				.Map(dest => dest.Title, src => src.Title)
				.Map(dest => dest.Introduction, src => src.Introduction)
				.Map(dest => dest.Content, src => src.Content)
				.Map(dest => dest.UrlSlug, src => src.UrlSlug)
				.Map(dest => dest.CoverImageUrl, src => src.CoverImageUrl)
				.Map(dest => dest.Author, src => src.Author)
				.Map(dest => dest.Category, src => src.Category)
				.Map(dest => dest.CreatedOn, src => src.CreatedOn)
				.Map(dest => dest.ModifiedOn, src => src.ModifiedOn)
				.Map(dest => dest.IsPublished, src => src.IsPublished)
				.Map(dest => dest.PublishedOn, src => src.PublishedOn)
				.Map(dest => dest.IsArchived, src => src.IsArchived)
				.Ignore(dest => dest.CanEdit)
				.IgnoreNullValues(true);

		TypeAdapterConfig<AppUserDto, AppUserDto>.NewConfig();
		TypeAdapterConfig<CategoryDto, CategoryDto>.NewConfig();
	}

}
