using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Michus.DAO
{
    public static class TokenStore
    {
        public static string? Token { get; set; }
        public static DateTime Expiration { get; set; }
    }

    public class CorreoHelper
    {
        private readonly string _correoOrigen;
        private readonly string _contraseñaCorreo;

        public CorreoHelper(IConfiguration configuration)
        {
            _correoOrigen = configuration["CorreoConfiguracion:CorreoOrigen"];
            _contraseñaCorreo = configuration["CorreoConfiguracion:ContraseñaCorreo"];
        }

        public async Task EnviarCorreoConTokenAsync(string destinatario)
        {
            // Genera un token de 5 dígitos
            string token = GenerarToken();

            // Almacena el token y la fecha de expiración (5 minutos desde ahora)
            TokenStore.Token = token;
            TokenStore.Expiration = DateTime.Now.AddMinutes(5);

            try
            {
                // Configura el mensaje de correo
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_correoOrigen);
                mail.To.Add(destinatario);
                mail.Subject = "Código de Acceso";
                mail.Body = $"MAURO SE LA COME, Aquí tienes tu código de acceso: {token}";

                // Configura el cliente SMTP
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(_correoOrigen, _contraseñaCorreo),
                    EnableSsl = true
                };

                // Envía el correo de forma asíncrona
                await smtpClient.SendMailAsync(mail);
                Console.WriteLine("Correo enviado exitosamente con el código de 5 dígitos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
                throw; // Re-lanzamos la excepción para que el controlador la capture
            }
        }

        private string GenerarToken()
        {
            Random random = new Random();
            return random.Next(10000, 99999).ToString(); // Genera un número aleatorio de 5 dígitos
        }

        public bool ValidarToken(string tokenIngresado)
        {
            // Verifica que el token coincida y que no haya expirado
            if (TokenStore.Token == tokenIngresado && DateTime.Now <= TokenStore.Expiration)
            {
                Console.WriteLine("Token válido.");
                return true;
            }
            else
            {
                Console.WriteLine("Token inválido o expirado.");
                return false;
            }
        }

    }
}