
namespace Web.Tests.Bunit.Pages
{
	public class HomePageTests : BunitContext
	{
		[Fact]
		public void HomePage_Should_RenderSuccessfully()
		{
			// Arrange & Act
			var cut = Render<Web.Components.Pages.Home>();

			// Assert
			cut.MarkupMatches("<h1>Home</h1>"); // Adjust to match expected markup
		}
	}
}
