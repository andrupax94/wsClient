using Moq;
using Xunit;

public class CalculadoraTests
{
    [Fact]
    public void Sumar_DosNumeros_DeberiaRetornarResultadoCorrecto()
    {
        // Arrange
        var calculadora = new Calculadora(null); // No necesitamos el mock para esta prueba
        int numero1 = 5;
        int numero2 = 3;

        // Act
        int resultado = calculadora.Sumar(numero1, numero2);

        // Assert
        Assert.Equal(8, resultado);
    }

    [Fact]
    public void GuardarMensaje_DeberiaLlamarGuardarEnRegistro()
    {
        // Arrange
        var mockRegistro = new Mock<IRegistro>();
        var calculadora = new Calculadora(mockRegistro.Object);
        string mensaje = "Hola, mundo!";

        // Act
        calculadora.GuardarMensaje(mensaje);

        // Assert
        mockRegistro.Verify(r => r.Guardar(mensaje), Times.Once); // Verifica que se haya llamado Guardar con el mensaje
    }
}