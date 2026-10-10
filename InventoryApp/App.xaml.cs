namespace InventoryApp;

public partial class App : Application
{
    public App()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            StartupException = ex;
        }
    }

    private Exception? StartupException { get; }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Page page;
        try
        {
            page = StartupException is null
                ? new MainPage()
                : CreateStartupErrorPage(StartupException);
        }
        catch (Exception ex)
        {
            page = CreateStartupErrorPage(ex);
        }

        return new Window(page);
    }

    private static ContentPage CreateStartupErrorPage(Exception exception)
    {
        var errorText = new Label
        {
            Text = exception.ToString(),
            FontSize = 12,
            TextColor = Colors.DarkRed,
            LineBreakMode = LineBreakMode.WordWrap
        };

        var retryButton = new Button { Text = "Retry opening Inventory" };
        retryButton.Clicked += (_, _) =>
        {
            try
            {
                var app = Current;
                if (app?.Windows.FirstOrDefault() is { } window)
                    window.Page = new MainPage();
            }
            catch (Exception retryException)
            {
                var app = Current;
                if (app?.Windows.FirstOrDefault() is { } window)
                    window.Page = CreateStartupErrorPage(retryException);
            }
        };

        var updateButton = new Button { Text = "Open latest app release" };
        updateButton.Clicked += async (_, _) =>
        {
            try
            {
                await Launcher.Default.OpenAsync(
                    "https://github.com/devrupeshgadkhe/MAUI-Project/releases/latest");
            }
            catch
            {
                // Keep the diagnostic page usable if the browser cannot be opened.
            }
        };

        return new ContentPage
        {
            Title = "Inventory startup error",
            BackgroundColor = Colors.White,
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 20,
                    Spacing = 12,
                    Children =
                    {
                        new Label
                        {
                            Text = "Inventory Management could not start",
                            FontSize = 22,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Black
                        },
                        new Label
                        {
                            Text = "The app opened its diagnostic screen because startup failed.",
                            TextColor = Colors.Black
                        },
                        retryButton,
                        updateButton,
                        new Label
                        {
                            Text = "Technical details",
                            FontSize = 16,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Black
                        },
                        errorText
                    }
                }
            }
        };
    }
}
