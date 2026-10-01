using WiFiAnalyzer.Core.UnitTests.Fakes;
using WiFiAnalyzer.Core.ViewModels.Networks;
using Xunit;

namespace WiFiAnalyzer.Core.UnitTests.ViewModels;

public class NetworksPageViewModelTests
{
    [Fact]
    public void Toggle_SwitchesBetweenTableAndGraph()
    {
        FakeNetworksService networksService = new();
        FakeNotificationService notificationService = new();
        NetworksPageViewModel viewModel = new(new(networksService, notificationService), new(networksService, notificationService));

        Assert.Same(viewModel.Table, viewModel.Current);
        Assert.Equal("View Graph", viewModel.ToggleLabel);

        viewModel.ToggleCommand.Execute(null);

        Assert.Same(viewModel.Graph, viewModel.Current);
        Assert.Equal("View Table", viewModel.ToggleLabel);
    }
}
