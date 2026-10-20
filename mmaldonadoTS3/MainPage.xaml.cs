using System.Globalization;
using System.Text.RegularExpressions;
using mmaldonadoTS3.Models;

namespace mmaldonadoTS3;

public partial class MainPage : ContentPage
{
    public MainPage() { InitializeComponent(); FechaPicker.MaximumDate = DateTime.Today; TipoPicker.SelectedIndex = 0; }

    private void TipoPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var tipo = TipoPicker.SelectedItem?.ToString();
        IdentificacionEntry.Keyboard = tipo is "CI" or "RUC" ? Keyboard.Numeric : Keyboard.Default;
        IdentificacionEntry.MaxLength = tipo switch { "CI" => 10, "RUC" => 13, _ => 20 };
        IdentificacionEntry.Placeholder = tipo switch { "CI" => "10 dígitos", "RUC" => "13 dígitos", _ => "Número de pasaporte" };
        IdentificacionEntry.Text = string.Empty;
    }

    private async void Continuar_Clicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        var tipo = TipoPicker.SelectedItem?.ToString();
        var id = IdentificacionEntry.Text?.Trim() ?? string.Empty;
        var nombres = NombresEntry.Text?.Trim() ?? string.Empty;
        var apellidos = ApellidosEntry.Text?.Trim() ?? string.Empty;
        var correo = CorreoEntry.Text?.Trim() ?? string.Empty;
        var salarioTexto = (SalarioEntry.Text ?? string.Empty).Trim().Replace(',', '.');
        string? error = ValidarIdentificacion(tipo, id);
        if (error is null && !NombreValido(nombres)) error = "Ingresa nombres válidos (letras, espacios, guiones o apóstrofes).";
        if (error is null && !NombreValido(apellidos)) error = "Ingresa apellidos válidos (letras, espacios, guiones o apóstrofes).";
        if (error is null && FechaPicker.Date > DateTime.Today) error = "La fecha no puede ser posterior a hoy.";
        if (error is null && !Regex.IsMatch(correo, @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$")) error = "Ingresa un correo electrónico válido.";
        if (error is null && (!decimal.TryParse(salarioTexto, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsed) || parsed <= 0)) error = "El salario debe ser un número mayor que cero.";
        if (error is not null) { ErrorLabel.Text = error; ErrorLabel.IsVisible = true; return; }
        decimal.TryParse(salarioTexto, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var salario);
        await Navigation.PushAsync(new Pages.ResumenPage(new Contacto(tipo!, id, nombres, apellidos, (FechaPicker.Date ?? DateTime.Today), correo, salario)));
    }

    private static string? ValidarIdentificacion(string? tipo, string id)
    {
        if (string.IsNullOrWhiteSpace(tipo)) return "Selecciona el tipo de identificación.";
        if (string.IsNullOrWhiteSpace(id)) return "Ingresa la identificación.";
        if (tipo == "CI" && !Regex.IsMatch(id, @"^\d{10}$")) return "La CI debe contener exactamente 10 números.";
        if (tipo == "RUC" && !Regex.IsMatch(id, @"^\d{13}$")) return "El RUC debe contener exactamente 13 números.";
        return null;
    }
    private static bool NombreValido(string valor) => !string.IsNullOrWhiteSpace(valor) && Regex.IsMatch(valor, @"^[\p{L}][\p{L}\s'-]*$");
}