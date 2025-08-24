using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Web.Components.Layout
{
    // Test-only component used to simulate exceptions from child content
    public class ThrowingChild : ComponentBase
    {
        [Parameter]
        public string? ThrowType { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (string.Equals(ThrowType, "Unauthorized", System.StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Test unauthorized");
            }

            if (string.Equals(ThrowType, "Argument", System.StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Test argument");
            }

            if (string.Equals(ThrowType, "NotFound", System.StringComparison.OrdinalIgnoreCase))
            {
                throw new KeyNotFoundException("Test not found");
            }

            if (string.Equals(ThrowType, "Generic", System.StringComparison.OrdinalIgnoreCase))
            {
                throw new System.Exception("Test generic");
            }

            builder.OpenElement(0, "div");
            builder.AddContent(1, "Child content rendered normally");
            builder.CloseElement();
        }
    }
}
