using Avalonia.Web.Blazor;

namespace SpiroNet.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<SpiroNet.App>()
            .SetupWithSingleViewLifetime();
    }
}
