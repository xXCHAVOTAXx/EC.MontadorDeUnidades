using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MontadorDeUnidades
{
    /// <summary>
    /// Montador de unidades de red para aplicaciones
    /// </summary>
    public class Montador
    {
        /// <summary>
        /// Conecta a una unidad de red asignandola a una letra dada
        /// </summary>
        /// <param name="letra">Letra a asignar a la unidad de red</param>
        /// <param name="direccionRemota">Dirección de la unidad de red. Ej. \\10.10.1.1, \\10.10.1.1\carpeta</param>
        /// <param name="usuario">Usuario para acceder a la unidad de red</param>
        /// <param name="contra">contraseña para acceder a la unidad de red</param>
        /// <returns>Devuelve un KeyValuePair con un bool si es correcto o no y un string para el mensaje en caso de error</returns>
        public KeyValuePair<bool, string> Conectar(string letra, string direccionRemota, string usuario, string contra)
        {
            try
            {
                string dispositivo = $@"{letra}:\";
                int? disponibilidadDispositivo = RevisaDispositivo(dispositivo);
                if (disponibilidadDispositivo != null)
                {
                    return new KeyValuePair<bool, string>(false, "El dispositivo esta ocupado");
                }
                string respuestaComando = EjecutaComando("net", $@"use {letra}: {direccionRemota} /user:{usuario} {contra} /p:yes");
                if (respuestaComando.StartsWith("Ok"))
                {
                    return new KeyValuePair<bool, string>(true, respuestaComando);
                }
                return new KeyValuePair<bool, string>(false, respuestaComando);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        /// <summary>
        /// Desconecta una unidad de red segun la letra dada
        /// </summary>
        /// <param name="letra">Letra de la unidad a desconectar</param>
        /// <returns>Devuelve un KeyValuePair con un bool si es correcto o no y un string para el mensaje en caso de error</returns>
        public KeyValuePair<bool, string> Desconectar(string letra)
        {
            try
            {
                string dispositivo = $@"{letra}:\";
                int? disponibilidadDispositivo = RevisaDispositivo(dispositivo);
                if (disponibilidadDispositivo == null || disponibilidadDispositivo != 4) // 4 = dispositivo de red
                {
                    if (disponibilidadDispositivo == null)
                    {
                        return new KeyValuePair<bool, string>(false, "La letra proporcionada no esta asignada");
                    }
                    return new KeyValuePair<bool, string>(false, "El dispositivo no es un dispositivo de red");
                }
                var respuestaComando = EjecutaComando("net", $"use /D {letra}:");
                if (respuestaComando == "Ok")
                {
                    return new KeyValuePair<bool, string>(true, respuestaComando);
                }
                return new KeyValuePair<bool, string>(false, respuestaComando);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        /// <summary>
        /// Revisa que el dispositivo dado este libre o como esta montado
        /// </summary>
        /// <param name="dispositivo">Dispositivo a revisar</param>
        /// <returns>Devuelve un int nulleable dependiendo del tipo de dispositivo encontrado</returns>
        private int? RevisaDispositivo(string dispositivo)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            DriveInfo dispositivoEncontrado = drives.Where(x => x.Name == dispositivo).FirstOrDefault();
            if (dispositivoEncontrado == null)
            {
                // El dispositivo esta libre
                return null;
            }
            // El dispositivo esta ocupado
            return (int)dispositivoEncontrado.DriveType;

            //DriveType tipoDispositivo = dispositivoEncontrado.DriveType;
            //switch (tipoDispositivo)
            //{
            //    case DriveType.Fixed:
            //        return "unidad montada en local";
            //    case DriveType.Network:
            //        return "unidad montada en red";
            //    case DriveType.Removable:
            //        return "unidad extraible";
            //    default:
            //        return "unidad de tipo indeterminado. Revisar los dispositivos del servidor";
            //    //case DriveType.CDRom:
            //    //    break;
            //    //case DriveType.NoRootDirectory:
            //    //    break;
            //    //case DriveType.Ram:
            //    //    break;
            //    //case DriveType.Unknown:
            //    //    break;
            //}
        }

        /// <summary>
        /// Ejecuta un comando como un cmd
        /// </summary>
        /// <param name="cmd">El comando a ejectuar</param>
        /// <param name="argv">Los argumentos u opciones del comando</param>
        /// <returns>Regresa lo que sucedio con el comando</returns>
        private static string EjecutaComando(string cmd, string argv)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = $" {argv}";
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                //p.WaitForExit();
                string output = p.StandardOutput.ReadToEnd();
                p.Dispose();
                return "Ok";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
                throw;
            }
        }
    }
}
