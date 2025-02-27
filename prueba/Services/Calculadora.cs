public interface IRegistro
{
    void Guardar(string mensaje);
}

public class Calculadora
{
    private readonly IRegistro _registro;

    public Calculadora(IRegistro registro)
    {
        _registro = registro;
    }

    // Método para sumar dos números
    public int Sumar(int a, int b)
    {
        return a + b;
    }

    // Método para guardar un mensaje utilizando la interfaz IRegistro
    public void GuardarMensaje(string mensaje)
    {
        _registro.Guardar(mensaje);
    }
}