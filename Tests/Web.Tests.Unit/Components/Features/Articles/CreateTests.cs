// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     CreateTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  Web.Tests.Bunit
// =======================================================

using Web.Components.Features.Articles.ArticleCreate;
using static Web.Components.Features.Articles.ArticleCreate.CreateArticle;
using Web.Data.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using NSubstitute;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

namespace Web.Components.Features.Articles;

/// <summary>
///   Unit tests for <see cref="Create" /> (Articles Create Page).
/// </summary>
[ExcludeFromCodeCoverage]
[TestSubject(typeof(Create))]
public class CreateTests : BunitContext
{

	private readonly ICreateArticleHandler _mockHandler;
	private readonly ILogger<Create> _mockLogger;

	public CreateTests()
	{
		_mockHandler = Substitute.For<ICreateArticleHandler>();
		_mockLogger = Substitute.For<ILogger<Create>>();
		Services.AddScoped(_ => _mockHandler);
		Services.AddScoped(_ => _mockLogger);
		// Removed registration for concrete Handler type; only register the interface for mocking
		Services.AddScoped<CreateArticle.ICreateArticleHandler>(_ => _mockHandler);
		Services.AddCascadingAuthenticationState();
		Services.AddAuthorization();
	}

	[Fact]
	public void Renders_Form()
	{
		// Arrange
		Helpers.SetAuthorization(this);
		var cut = Render<Create>();

		// Act
		// (No action needed, render)

		// Assert
		cut.Markup.Should().Contain("Title");
		cut.Markup.Should().Contain("Introduction");
	}

	[Fact]
	public void Renders_Form_With_All_Fields()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var cut = Render<Create>();

		// Assert
		cut.Markup.Should().Contain("Title");
		cut.Markup.Should().Contain("Introduction");
		cut.Markup.Should().Contain("Content");
		cut.Markup.Should().Contain("Cover Image URL");
		cut.Markup.Should().Contain("Published");
	}

	[Fact]
	public void Shows_Validation_Errors_When_Form_Is_Invalid()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Act
		form.Submit();

		// Assert
		cut.Markup.Should().Contain("validation-message");
	}

	[Fact]
	public async Task Submits_Form_And_Calls_Handler_When_Valid()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(Task.FromResult(Result.Ok()));
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Fill in required fields
		cut.Find("input[name='_article.Title']").Change("Test");
		cut.Find("input[name='_article.Introduction']").Change("Intro");
		cut.Find("textarea[name='_article.Content']").Change("Content");
		cut.Find("input[name='_article.CoverImageUrl']").Change("https://img.com/test.jpg");
		cut.Find("input[name='_article.UrlSlug']").Change("test_article");

		// Act
		await form.SubmitAsync();

		// Assert
		await _mockHandler.Received(1).HandleAsync(Arg.Any<ArticleDto>());
	}

	[Fact]
	public async Task Shows_Error_Message_When_Handler_Fails()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(Task.FromResult(Result.Fail("Error occurred")));
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Fill in required fields
		cut.Find("input[name='_article.Title']").Change("Test");
		cut.Find("input[name='_article.Introduction']").Change("Intro");
		cut.Find("textarea[name='_article.Content']").Change("Content");
		cut.Find("input[name='_article.CoverImageUrl']").Change("https://img.com/test.jpg");
		cut.Find("input[name='_article.UrlSlug']").Change("test_article");

		// Act
		await form.SubmitAsync();

		// Assert
		cut.Markup.Should().Contain("alert-danger");
	}

	[Fact]
	public async Task Navigates_To_Article_List_On_Success()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(Task.FromResult(Result.Ok()));
		var nav = Services.GetRequiredService<NavigationManager>();
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Fill in required fields
		cut.Find("input[name='_article.Title']").Change("Test");
		cut.Find("input[name='_article.Introduction']").Change("Intro");
		cut.Find("textarea[name='_article.Content']").Change("Content");
		cut.Find("input[name='_article.CoverImageUrl']").Change("https://img.com/test.jpg");
		cut.Find("input[name='_article.UrlSlug']").Change("test_article");

		// Act
		await form.SubmitAsync();

		// Assert
		nav.Uri.Should().EndWith("/articles");
	}

	[Fact]
	public async Task Submit_Button_Disabled_While_Submitting()
	{
		// Arrange
		Helpers.SetAuthorization(this, true, "Admin");
		var tcs = new TaskCompletionSource<Result>();
		_mockHandler.HandleAsync(Arg.Any<ArticleDto>()).Returns(tcs.Task);
		var cut = Render<Create>();
		var form = cut.Find("form");

		// Fill in required fields
		cut.Find("input[name='_article.Title']").Change("Test");
		cut.Find("input[name='_article.Introduction']").Change("Intro");
		cut.Find("textarea[name='_article.Content']").Change("Content");
		cut.Find("input[name='_article.CoverImageUrl']").Change("https://img.com/test.jpg");
		cut.Find("input[name='_article.UrlSlug']").Change("test_article");

		// Act
		var submitButton = cut.Find("button[type='submit']");
		var submitTask = form.SubmitAsync();

		// Assert
		submitButton.HasAttribute("disabled").Should().BeTrue();
		// Complete the handler to finish submission
		tcs.SetResult(Result.Ok());
		await submitTask;
	}

	[Fact(Skip = "Bunit does not enforce [Authorize]; form is always rendered in test context.")]
	public void Only_Admin_Or_Author_Can_Access()
	{
		// Arrange
		Helpers.SetAuthorization(this, false);

		// Act
		var cut = Render<Create>();

		// Assert
		cut.FindAll("form").Should().BeEmpty(); // No form should be rendered for unauthorized users
	}

}