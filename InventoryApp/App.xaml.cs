namespace InventoryApp;

public partial class App : Application
{
    public App()
    {
        try
        {
            InitializeComponent();
            MainPage = new MainPage();
        }
        catch (Exception ex)
        {
            // Never leave the user with a blank screen if startup/XAML initialization fails.
            MainPage = CreateStartupErrorPage(ex);
        }
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
                Current!.MainPage = new MainPage();
            }
            catch (Exception retryException)
            {
                Current!.MainPage = CreateStartupErrorPage(retryException);
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
                            Text = "The startup error is shown below so the app does not stay on a blank screen.",
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
