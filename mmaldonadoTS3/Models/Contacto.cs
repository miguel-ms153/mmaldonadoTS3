namespace mmaldonadoTS3.Models;

public sealed record Contacto(string TipoIdentificacion, string Identificacion, string Nombres, string Apellidos, DateTime Fecha, string Correo, decimal Salario)
{
    public const decimal AportePersonalPorcentaje = 0.0945m;
    public const decimal AportePatronalPorcentaje = 0.1115m;
    public decimal AportePersonal => Math.Round(Salario * AportePersonalPorcentaje, 2, MidpointRounding.AwayFromZero);
    public decimal AportePatronal => Math.Round(Salario * AportePatronalPorcentaje, 2, MidpointRounding.AwayFromZero);
    public decimal SalarioNeto => Salario - AportePersonal;
}