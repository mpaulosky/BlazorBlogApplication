
namespace Web.Tests.Bunit.Components.Features
{
	public class ArticleListTests : BunitContext
	{
		[Fact]
		public void ArticleList_Should_RenderList()
		{
			// Arrange & Act
			var cut = Render<Web.Components.Features.Articles.ArticleList.List>();

			// Assert
			cut.MarkupMatches("<article>"); // Adjust to match expected markup
		}
	}
}
