// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     DetailsTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  Web.Tests.Bunit
// =======================================================

using Web.Components.Features.Categories.CategoryDetails;
using NSubstitute;
using System.Threading.Tasks;

namespace Web.Components.Features.Categories;

/// <summary>
///   Unit tests for <see cref="Categories.CategoryGet.Details" />
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Details))]
public class DetailsTests : BunitContext
{
	private readonly IValidator<CategoryDto> _categoryDtoValidator = Substitute.For<IValidator<CategoryDto>>();

	public DetailsTests()
	{
		Services.AddScoped<ILogger<Details>, Logger<Details>>();
		Services.AddScoped(_ => _categoryDtoValidator);
		Services.AddCascadingAuthenticationState();
	}

	[Fact]
	public void RendersNotFound_WhenCategoryIsNull()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<MongoDB.Bson.ObjectId>()).Returns(Task.FromResult(Web.Data.Abstractions.Result.Fail<Web.Data.Models.CategoryDto>("Category not found.")));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, ObjectId.GenerateNewId()));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");
	}

	[Fact]
	public void RendersCategoryDetails_WhenCategoryIsPresent()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Is<MongoDB.Bson.ObjectId>(id => id == categoryDto.Id)).Returns(Task.FromResult(Web.Data.Abstractions.Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain(categoryDto.CategoryName);
		cut.Markup.Should().Contain("Created On: 1/1/2025");
		cut.Markup.Should().Contain("Modified On: 1/1/2025");
		cut.Find("button.btn-secondary").Should().NotBeNull();
		cut.Find("button.btn-light").Should().NotBeNull();
	}

	[Fact]
	public void HasCorrectNavigationButtons()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<MongoDB.Bson.ObjectId>()).Returns(Task.FromResult(Web.Data.Abstractions.Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Find("button.btn-secondary").Should().NotBeNull();
		cut.Find("button.btn-light").Should().NotBeNull();
	}

	[Fact]
	public void NavigatesToEditPage_WhenEditButtonClicked()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<MongoDB.Bson.ObjectId>()).Returns(Task.FromResult(Web.Data.Abstractions.Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
		var navigationManager = Services.GetRequiredService<BunitNavigationManager>();

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		cut.Find("button.btn-secondary").Click();

		// Assert
		navigationManager.Uri.Should().EndWith($"/categories/edit/{categoryDto.Id}");
	}

	[Fact]
	public void NavigatesToListPage_WhenBackButtonClicked()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var categoryDto = FakeCategoryDto.GetNewCategoryDto(true);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<MongoDB.Bson.ObjectId>()).Returns(Task.FromResult(Web.Data.Abstractions.Result.Ok(categoryDto)));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);
		var navigationManager = Services.GetRequiredService<BunitNavigationManager>();

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, categoryDto.Id));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));
		cut.Find("button.btn-light").Click();

		// Assert
		navigationManager.Uri.Should().EndWith("/categories");
	}

	[Fact]
	public void HandlesEmptyObjectId()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<MongoDB.Bson.ObjectId>()).Returns(Task.FromResult(Web.Data.Abstractions.Result.Fail<Web.Data.Models.CategoryDto>("Category not found.")));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, ObjectId.Empty));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");
	}

	[Fact]
	public void HandlesServiceException_Gracefully()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var getSub = Substitute.For<GetCategory.IGetCategoryHandler>();
		getSub.HandleAsync(Arg.Any<MongoDB.Bson.ObjectId>()).Returns<Task<Result<CategoryDto>>>(_ => throw new Exception("DB error"));
		Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getSub);

		// Act
		var cut = Render<Details>(parameters => parameters.Add(p => p.Id, ObjectId.GenerateNewId()));
		cut.WaitForState(() => !cut.Markup.Contains("animate-spin"), TimeSpan.FromSeconds(5));

		// Assert
		cut.Markup.Should().Contain("Category not found");
	}

}