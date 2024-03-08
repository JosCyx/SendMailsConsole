using System;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SendMailsConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            string jsonText = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Config>(jsonText);

            // Si no se proporcionan los argumentos esperados, mostrar un mensaje de ayuda
            if (args.Length != 4 || args[0] != "-d" || args[2] != "-a")
            {
                Console.WriteLine("Instrucciones de ejecucion: app.exe -d destinatario@gmail.com -a \"C:\\carpeta\\archivo.pdf\"");
                
                return;
            }

            // Obtener los valores de los argumentos
            string destinatario = args[1];
            string filePath = args[3];

            try
            {
                var emailUsername = config.Email;//CORREO EMISOR
                var emailPass = config.Password;//CONTRASEÑA CORREO EMISOR 
                var asunto = config.Subject;//ASUNTO DEL CORREO

                var email = new MimeMessage();//CREA UN OBJETO MIME MESSAGE 
                email.From.Add(MailboxAddress.Parse(emailUsername));//ASIGNA EL CORREO EMISOR AL OBJETO EMAIL
                email.To.Add(MailboxAddress.Parse(destinatario));// ASIGNA EL CORREO DESTINATARIO AL OBJETO EMAIL
                email.Subject = asunto;// ASIGNA EL ASUNTO AL OBJETO EMAIL

                var builder = new BodyBuilder();//CREAR UN OBJETO BODYBUILDER PARA EL CUERPO DEL CORREO
                builder.Attachments.Add(filePath);//ADJUNTAR EL ARCHIVO AL CUERPO DEL CORREO
                //builder.HtmlBody = cuerpoHTML;// ASIGNA EL CUERPO EN FORMATO HTML AL BODYBUILDER
                email.Body = builder.ToMessageBody();//ASIGNAR EL BUILDER Y SU CONTENIDO AL CUERPO DEL CORREO

                var smtp = new SmtpClient();// CREAR UN OBJETO SMTPCLIENT PARA CONECTARSE AL SERVIDOR SMTP
                smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);// DEFINIR A QUE SERVIDOR SE CONECTARA Y EL PUERTO
                smtp.Authenticate(emailUsername, emailPass);// PASAR LAS CREDENCIALES DE AUTENTICACION, USUARIO Y CONTRASEÑA EMISOR
                smtp.Send(email);//ENVIA EL CORREO
                smtp.Disconnect(true);//ELIMINA LA CONEXION SMTP

                Console.WriteLine("Correo enviado correctamente.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
        }
    }

    class Config
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
    }
}
