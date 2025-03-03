using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using prueba.Pages.ConectarComponent;
namespace test.bUnit;
public class ConectarComponentTest : TestContext
{
    [Fact]
    public async Task ConectarComponent_ShouldRenderCorrectly()
    {
        // 1️⃣ Crear IConfiguration de prueba
        var configuration = new ConfigurationService().GetConfiguration();

        // 2️⃣ Crear instancia de StatusMessageService
        var statusMessageService = new StatusMessageService();

        // 3️⃣ Crear instancia real de SignalRService
        var signalRService = new SignalRService(configuration, statusMessageService);

        // 4️⃣ Agregar las dependencias al contexto de pruebas
        Services.AddSingleton<IConfiguration>(configuration);
        Services.AddSingleton(statusMessageService);
        Services.AddSingleton(signalRService);

        // 5️⃣ Renderizar el componente
        var cut = RenderComponent<Conectar>();

        // 6️⃣ Verificar que se renderizó correctamente
        Assert.NotNull(cut.Find("#main"));

        // 7️⃣ Probar la conexión (opcional)
        await signalRService.Connect();
        Assert.NotNull(signalRService);
    }
}
