
namespace Web.Tests.Bunit.Components.Layout
{
	public class MainLayoutTests : BunitContext
	{
		[Fact]
		public void MainLayout_Should_RenderNavMenu()
		{
			// Arrange & Act
			var cut = Render<Web.Components.Layout.MainLayout>();

			// Assert
			cut.MarkupMatches("<nav>"); // Adjust to match expected markup
		}
	}
}
