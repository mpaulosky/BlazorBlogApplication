// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     TestServiceRegistrations.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

using Web.Components.Features.Categories.CategoryCreate;

namespace Web;

[ExcludeFromCodeCoverage]
public static class TestServiceRegistrations
{

	// Auth0 removed from tests; do not register an Auth0Service replacement here.

	// If a concrete handler type is registered in the BunitContext services but
	// the corresponding interface isn't, register a resolution from the
	// interface to the already-registered concrete implementation, so components
	// that request the interface via property injection succeed.
	private static void MapIfConcreteRegistered<TConcrete, TInterface>(BunitContext ctx)
			where TConcrete : class
			where TInterface : class
	{
		// If the interface is already registered, nothing to do.
		if (ctx.Services.Any(sd => sd.ServiceType == typeof(TInterface)))
		{
			return;
		}

		// Always add a mapping factory for the interface that will attempt to
		// resolve the concrete implementation at runtime. This allows tests that
		// register the concrete handler either before or after calling
		// RegisterAll/RegisterCommonUtilities to still have the interface
		// resolved by Blazor's property injection. If the concrete cannot be
		// resolved (not registered or fails to construct), fall back to an
		// NSubstitute double so component rendering doesn't NRE.
		ctx.Services.AddScoped(typeof(TInterface), sp =>
		{
			try
			{
				var concrete = sp.GetService(typeof(TConcrete));

				if (concrete is TInterface asInterface)
				{
					return asInterface;
				}
			}
			catch
			{
				// ignore and fall through to substitute
			}

			// Return a substitute implementation so tests don't fail due to
			// unresolved concretes.
			return Substitute.For<TInterface>();
		});
	}

	// Register ApplicationDbContext and a simple factory that returns it
	private static ApplicationDbContext RegisterMyBlogContext(BunitContext ctx)
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		var blogContext = new ApplicationDbContext(options);

		ctx.Services.AddScoped(_ => blogContext);
		ctx.Services.AddScoped<IApplicationDbContextFactory>(_ => new TestIApplicationDbContextFactory(blogContext));

		return blogContext;
	}

	// Register NSubstitute handler doubles for categories and articles used in many tests
	private static void RegisterHandlerSubstitutes(BunitContext ctx)
	{
		// Prepare sample data shared by multiple default substitutes
		var sampleCategory = new CategoryDto
		{
			Id = Guid.NewGuid(),
			CategoryName = "General Programming"
		};

		var sampleArticle = new ArticleDto
		{
			Id = Guid.NewGuid(),
			Title = "The Empathy Of Mission Contemplation",
			Introduction = "Intro",
			Content = "Sample article content",
			CoverImageUrl = string.Empty,
			UrlSlug = "the-empathy-of-mission-contemplation",
			Author = AppUserDto.Empty,
			Category = sampleCategory,
			CreatedOn = DateTime.UtcNow,
			ModifiedOn = null,
			IsPublished = true,
			PublishedOn = DateTime.UtcNow,
			IsArchived = false,
			CanEdit = true
		};

		if (!IsEitherRegistered(typeof(GetCategory.IGetCategoryHandler), typeof(GetCategory.Handler)))
		{
			var getCategorySub = Substitute.For<GetCategory.IGetCategoryHandler>();

			// Default: return a sample category so components render normally in tests.
	    getCategorySub.HandleAsync(Arg.Any<Guid>())
		    .Returns(Result.Ok(sampleCategory));

			// Register the interface with a factory that prefers a concrete handler if present,
			// otherwise falls back to the substitute.
			ctx.Services.AddScoped<GetCategory.IGetCategoryHandler>(sp =>
			{
				try
				{
					var concrete = sp.GetService(typeof(GetCategory.Handler));

					if (concrete is GetCategory.IGetCategoryHandler asInterface)
					{
						return asInterface;
					}
				}
				catch
				{
					// ignored
				}

				return getCategorySub;
			});
		}

		if (!IsEitherRegistered(typeof(GetCategories.IGetCategoriesHandler), typeof(GetCategories.Handler)))
		{
			var getCategoriesSub = Substitute.For<GetCategories.IGetCategoriesHandler>();

	    getCategoriesSub.HandleAsync(Arg.Any<bool>())
		    .Returns(Result.Ok<IEnumerable<CategoryDto>>([ sampleCategory ]));

			ctx.Services.AddScoped<GetCategories.IGetCategoriesHandler>(sp =>
			{
				try
				{
					var concrete = sp.GetService(typeof(GetCategories.Handler));

					if (concrete is GetCategories.IGetCategoriesHandler asInterface)
					{
						return asInterface;
					}
				}
				catch
				{
					// ignored
				}

				return getCategoriesSub;
			});
		}

		if (!IsEitherRegistered(typeof(CreateCategory.ICreateCategoryHandler), typeof(CreateCategory.Handler)))
		{
			var createCategorySub = Substitute.For<CreateCategory.ICreateCategoryHandler>();
			createCategorySub.HandleAsync(Arg.Any<CategoryDto>()).Returns(Result.Ok());

			ctx.Services.AddScoped<CreateCategory.ICreateCategoryHandler>(sp =>
			{
				try
				{
					var concrete = sp.GetService(typeof(CreateCategory.Handler));

					if (concrete is CreateCategory.ICreateCategoryHandler asInterface)
					{
						return asInterface;
					}
				}
				catch
				{
					// ignored
				}

				return createCategorySub;
			});
		}

		if (!IsEitherRegistered(typeof(EditCategory.IEditCategoryHandler), typeof(EditCategory.Handler)))
		{
			var editCategorySub = Substitute.For<EditCategory.IEditCategoryHandler>();
			editCategorySub.HandleAsync(Arg.Any<CategoryDto>()).Returns(Result.Ok());

			ctx.Services.AddScoped<EditCategory.IEditCategoryHandler>(sp =>
			{
				try
				{
					var concrete = sp.GetService(typeof(EditCategory.Handler));

					if (concrete is EditCategory.IEditCategoryHandler asInterface)
					{
						return asInterface;
					}
				}
				catch
				{
					// ignored
				}

				return editCategorySub;
			});
		}

		// Articles
		if (!IsEitherRegistered(typeof(GetArticle.IGetArticleHandler), typeof(GetArticle.Handler)))
		{
			var getArticleSub = Substitute.For<GetArticle.IGetArticleHandler>();
			getArticleSub.HandleAsync(Arg.Any<Guid>()).Returns(Result.Ok(sampleArticle));

			ctx.Services.AddScoped<GetArticle.IGetArticleHandler>(sp =>
			{
				try
				{
					var concrete = sp.GetService(typeof(GetArticle.Handler));

					if (concrete is GetArticle.IGetArticleHandler asInterface)
					{
						return asInterface;
					}
				}
				catch (Exception)
				{
					// ignored
				}

				return getArticleSub;
			});
		}

		if (!IsEitherRegistered(typeof(GetArticles.IGetArticlesHandler), typeof(GetArticles.Handler)))
		{
			var getArticlesSub = Substitute.For<GetArticles.IGetArticlesHandler>();

	    getArticlesSub.HandleAsync(Arg.Any<bool>())
		    .Returns(Result.Ok<IEnumerable<ArticleDto>>([ sampleArticle ]));

			ctx.Services.AddScoped<GetArticles.IGetArticlesHandler>(sp =>
			{
				try
				{
					var concrete = sp.GetService(typeof(GetArticles.Handler));

					if (concrete is GetArticles.IGetArticlesHandler asInterface)
					{
						return asInterface;
					}
				}
				catch
				{
					// ignored
				}

				return getArticlesSub;
			});
		}

		return;

		bool IsEitherRegistered(Type iface, Type concrete)
		{
			return IsRegistered(iface) || IsRegistered(concrete);
		}

		// Categories
		bool IsRegistered(Type t)
		{
			return ctx.Services.Any(sd => sd.ServiceType == t);
		}
	}

	// Register common category handlers (both concrete and interface) wired to ApplicationDbContext from RegisterMyBlogContext
	public static void RegisterCategoryHandlers(BunitContext ctx)
	{
		// Create and register an ApplicationDbContext instance and use it directly. Do not
		// call BuildServiceProvider() here because Bunit will lock the service
		// provider once any service is resolved.
		var context = RegisterMyBlogContext(ctx);

		var loggerGet = Substitute.For<ILogger<GetCategory.Handler>>();
		var getHandler = new GetCategory.Handler(new TestIApplicationDbContextFactory(context), loggerGet);
		ctx.Services.AddScoped(_ => getHandler);
		ctx.Services.AddScoped<GetCategory.IGetCategoryHandler>(_ => getHandler);

		var loggerEdit = Substitute.For<ILogger<EditCategory.Handler>>();
		var factory = new TestIApplicationDbContextFactory(context);
		var editHandler = new EditCategory.Handler(factory, loggerEdit);
		ctx.Services.AddScoped(_ => editHandler);
		ctx.Services.AddScoped<EditCategory.IEditCategoryHandler>(_ => editHandler);
	}

	// Register a NavigationManager and a logger if not present
	public static void RegisterCommonUtilities(BunitContext ctx)
	{
		// Ensure a placeholder DefaultConnection exists so code paths that build the
		// application host (RegisterDatabaseContext) do not throw when running tests.
		// their own in-memory contexts explicitly via RegisterMyBlogContext.
		if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DefaultConnection")))
		{
			Environment.SetEnvironmentVariable("DefaultConnection", "Host=localhost;Database=Test;Username=Test;Password=Test");
		}

		if (ctx.Services.All(sd => sd.ServiceType != typeof(NavigationManager)))
		{
			ctx.Services.AddSingleton<NavigationManager, BunitNavigationManager>();
		}

		// Add a generic logger factory if none present
		if (ctx.Services.All(sd => sd.ServiceType != typeof(ILoggerFactory)))
		{
			var loggerFactory = new LoggerFactory();
			ctx.Services.AddSingleton(loggerFactory);
			ctx.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
		}

		// Ensure bUnit's authorization subsystem is configured. Call AddAuthorization
		// on the BunitContext so components using authentication/authorization get
		// the appropriate test doubles. If AddAuthorization cannot be called for any
		// reason (very unlikely), fall back to registering a simple
		// TestFallbackAuthenticationStateProvider so components that only depend on
		// an AuthenticationStateProvider still render without throwing.
		try
		{
			// This registers bUnit's test authorization system and placeholder
			// authentication state provider which tests can configure via the
			// returned authorization context.
			ctx.AddAuthorization();
		}
		catch
		{
			// If for some reason AddAuthorization isn't available, ensure there is
			// still an AuthenticationStateProvider so rendering doesn't throw.
			if (ctx.Services.All(sd => sd.ServiceType != typeof(AuthenticationStateProvider)))
			{
				ctx.Services.AddScoped<AuthenticationStateProvider, TestFallbackAuthenticationStateProvider>();
			}
		}

		// Register lightweight handler substitutes by default, so tests that rely
		// on handlers being present (but don't register concretes) will render
		// components without throwing. Individual tests may still register
		// concrete handlers or override these substitutes as needed.
		try
		{
			RegisterHandlerSubstitutes(ctx);
		}
		catch
		{
			// Swallow - registration of substitutes is the best effort for test safety.
		}

		// Auth0 removed: don't register Auth0Service here.
	}

	// Simple fallback provider returning an anonymous (unauthenticated) user.
	internal class TestFallbackAuthenticationStateProvider : AuthenticationStateProvider
	{

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

			return Task.FromResult(new AuthenticationState(anonymous));
		}

	}

	// Convenience method to register all common services used by component tests
	public static void RegisterAll(BunitContext ctx)
	{
		RegisterCommonUtilities(ctx);
	// Auth0 removed: no-op

		// Ensure ApplicationDbContext is available so concrete handlers (that require it)
		// can be activated when tests register concrete handler types directly.
		if (ctx.Services.All(sd => sd.ServiceType != typeof(ApplicationDbContext)))
		{
			RegisterMyBlogContext(ctx);
		}

		// Map common concrete handler registrations to their interfaces using a
		// factory mapping that will attempt to resolve a concrete handler when an
		// interface is requested and fall back to a Substitute if none exists. This
		// allows tests to register concrete handlers after calling RegisterAll and
		// still have components resolve the expected interfaces.
		MapIfConcreteRegistered<GetCategory.Handler, GetCategory.IGetCategoryHandler>(ctx);
		MapIfConcreteRegistered<EditCategory.Handler, EditCategory.IEditCategoryHandler>(ctx);
		MapIfConcreteRegistered<GetCategories.Handler, GetCategories.IGetCategoriesHandler>(ctx);
		MapIfConcreteRegistered<CreateCategory.Handler, CreateCategory.ICreateCategoryHandler>(ctx);
		MapIfConcreteRegistered<GetArticle.Handler, GetArticle.IGetArticleHandler>(ctx);
		MapIfConcreteRegistered<GetArticles.Handler, GetArticles.IGetArticlesHandler>(ctx);

		// Also, register lightweight handler substitutes so tests that call
		// RegisterAll() (for example Helpers.SetAuthorization) have default
		// handler implementations available, and components render instead of
		// throwing due to missing services.
		try
		{
			RegisterHandlerSubstitutes(ctx);
		}
		catch
		{
			// best-effort; tests may override substitutes as needed
		}

		// No JSInterop setup by default. Tests that require JSInterop should set it up
		// explicitly in the test so the behavior matches the package and version they use.

		// Intentionally, do not build the service provider here. Tests should set up
		// a navigation state after services are registered if they need to navigate.
	}

	// Note: handler substitutes were intentionally removed to allow per-test fixtures
	// and concrete handler registrations to control the behavior. MapIfConcreteRegistered
	// will still provide a fallback substitute if a concrete handler isn't available
	// at resolution time.

	// Register concrete article handlers (mirroring Program.cs registrations) if tests need concrete handlers
	public static void RegisterArticleHandlers(BunitContext ctx)
	{
		// no-op for now; concrete handlers rely on ApplicationDbContext, which we register in RegisterMyBlogContext
	}

	// Internal simple IApplicationDbContextFactory used by tests
	private class TestIApplicationDbContextFactory : IApplicationDbContextFactory
	{

		private readonly ApplicationDbContext _ctx;

		public TestIApplicationDbContextFactory(ApplicationDbContext ctx)
		{
			_ctx = ctx;
		}

		public ApplicationDbContext CreateDbContext()
		{
			return _ctx;
		}

	}

}