using System.Globalization;
using System.Text;
using mmaldonadoTS3.Models;

namespace mmaldonadoTS3.Pages;

public partial class ResumenPage : ContentPage
{
    private readonly Contacto _contacto;
    public ResumenPage(Contacto contacto)
    {
        InitializeComponent(); _contacto = contacto;
        var moneda = CultureInfo.GetCultureInfo("es-EC");
        IdentificacionLabel.Text = $"{contacto.TipoIdentificacion}: {contacto.Identificacion}";
        FechaLabel.Text = contacto.Fecha.ToString("dd/MM/yyyy", moneda);
        NombresLabel.Text = contacto.Nombres; ApellidosLabel.Text = contacto.Apellidos; CorreoLabel.Text = contacto.Correo;
        SalarioLabel.Text = contacto.Salario.ToString("C2", moneda);
        AportePersonalLabel.Text = contacto.AportePersonal.ToString("C2", moneda);
        NetoLabel.Text = contacto.SalarioNeto.ToString("C2", moneda);
        AportePatronalLabel.Text = contacto.AportePatronal.ToString("C2", moneda);
    }

    private async void Exportar_Clicked(object? sender, EventArgs e)
    {
        try
        {
            var moneda = CultureInfo.GetCultureInfo("es-EC");
            var texto = new StringBuilder().AppendLine("RESUMEN DE CONTACTO").AppendLine("===================")
                .AppendLine($"Tipo de identificación: {_contacto.TipoIdentificacion}").AppendLine($"Identificación: {_contacto.Identificacion}")
                .AppendLine($"Nombres: {_contacto.Nombres}").AppendLine($"Apellidos: {_contacto.Apellidos}")
                .AppendLine($"Fecha: {_contacto.Fecha:dd/MM/yyyy}").AppendLine($"Correo: {_contacto.Correo}")
                .AppendLine($"Salario mensual: {_contacto.Salario.ToString("C2", moneda)}")
                .AppendLine($"Aporte personal IESS (9,45 %): {_contacto.AportePersonal.ToString("C2", moneda)}")
                .AppendLine($"Salario luego del aporte personal: {_contacto.SalarioNeto.ToString("C2", moneda)}")
                .AppendLine($"Aporte patronal referencial (11,15 %): {_contacto.AportePatronal.ToString("C2", moneda)}")
                .AppendLine().AppendLine("Referencia: IESS, trabajador del sector privado bajo relación de dependencia.").ToString();
            var idSeguro = string.Concat(_contacto.Identificacion.Where(char.IsLetterOrDigit));
            var ruta = Path.Combine(FileSystem.CacheDirectory, $"contacto-{idSeguro}-{DateTime.Now:yyyyMMdd-HHmmss}.txt");
            await File.WriteAllTextAsync(ruta, texto, Encoding.UTF8);
            await Share.Default.RequestAsync(new ShareFileRequest { Title = "Exportar resumen del contacto", File = new ShareFile(ruta, "text/plain") });
        }
        catch (Exception ex) { await DisplayAlertAsync("No se pudo exportar", $"Ocurrió un error al crear el archivo: {ex.Message}", "Cerrar"); }
    }
    private async void NuevoContacto_Clicked(object? sender, EventArgs e) => await Navigation.PopToRootAsync();
}