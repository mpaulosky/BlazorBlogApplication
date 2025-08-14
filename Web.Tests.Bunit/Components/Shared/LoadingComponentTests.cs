
namespace Web.Tests.Bunit.Components.Shared
{
	public class LoadingComponentTests : BunitContext
	{
		[Fact]
		public void LoadingComponent_Should_RenderSpinner()
		{
			// Arrange & Act
			var cut = Render<Web.Components.Shared.LoadingComponent>();

			// Assert
			cut.MarkupMatches("<div>loading</div>"); // Adjust to match expected markup
		}
	}
}
