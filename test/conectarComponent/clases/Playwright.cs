using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

public class BaseTest : IAsyncLifetime
{
    protected IPage Page { get; private set; }
    protected IBrowser Browser { get; private set; }
    protected IBrowserContext Context { get; private set; }

    // Configuración común para todas las pruebas
    public async Task InitializeAsync()
    {
        // Inicializar Playwright
        var playwright = await Playwright.CreateAsync();

        // Ruta al ejecutable de Chrome en tu máquina
        var chromeExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; // Cambia esto a la ruta de tu Chrome

        // Lanzar el navegador con la ruta especificada
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false, // Si deseas ver el navegador en acción
            ExecutablePath = chromeExecutablePath // Ruta al ejecutable de Chrome
        });

        // Crear un contexto de navegador (puedes usar contextos para aislamiento de pruebas)
        Context = await Browser.NewContextAsync();

        // Abrir una página nueva
        Page = await Context.NewPageAsync();

        // Navegar a la URL principal que se va a probar

    }

    // Cerrar el navegador después de cada prueba
    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();
    }
}
