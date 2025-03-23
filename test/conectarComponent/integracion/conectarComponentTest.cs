using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.Playwright;
using Moq;
using prueba.Pages.Conectar;
using System.Threading.Tasks;
namespace test.bUnit;
public abstract class TestBase : TestContext // Hereda de TestContext de bUnit
{
    protected IRenderedComponent<Conectar> Renderizar()
    {
        // Crear configuración de prueba
        var configuration = new ConfigurationService().GetConfiguration();
        var statusMessageService = new StatusMessageService();
        var signalRService = new SignalRService(configuration, statusMessageService);
        var jsInteropMock = new Mock<IJSRuntime>();
        Services.AddSingleton(jsInteropMock.Object);
        // Agregar dependencias al contenedor de servicios de prueba
        Services.AddSingleton<IConfiguration>(configuration);
        Services.AddSingleton(statusMessageService);
        Services.AddSingleton(signalRService);

        // Renderizar el componente
        return RenderComponent<Conectar>();
    }
}
public class ConectarComponentTest : TestBase
{
    [Fact]
    public async Task RenderizarSolo()
    {
        // Usar el método heredado Renderizar()
        var cut = Renderizar();

        // Verificar que el componente se renderizó correctamente
        Assert.NotNull(cut.Find("#main"));
    }
    [Fact]
    public async Task RenderizarYConectar()
    {
        // Usar el método heredado Renderizar()
        var cut = Renderizar();

        // Verificar que el componente se renderizó correctamente
        Assert.NotNull(cut.Find("#main"));

        // Acceder a SignalRService desde los servicios de prueba
        var signalRService = Services.GetService<SignalRService>();
        Assert.NotNull(signalRService);

        // Probar la conexión
        await signalRService.Connect();
    }
    [Fact]
    public async Task ExistenciaInputs()
    {
        // Usar el método heredado Renderizar()
        var cut = Renderizar();


        var input1 = cut.Find("select[data-testid='Host']");
        var input2 = cut.Find("input[data-testid='N° Conexiones']");
        var input3 = cut.Find("input[data-testid='Hub']");
        var input4 = cut.Find("button[data-testid='Limpiar y Conectar']");
        var input5 = cut.Find("button[data-testid='Conectar y Agregar']");
        Assert.NotNull(input1);
        Assert.NotNull(input2);
        Assert.NotNull(input3);
        Assert.NotNull(input4);
        Assert.NotNull(input5);

    }

}



