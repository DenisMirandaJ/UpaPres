using System;
using System.Collections.Generic;
using Core.Controllers;
using Core.Models;

namespace Core
{
    /// <summary>
    /// 
    /// </summary>
    public class App
    {
        /// <summary>
        /// Punto de entrada de la aplicacion.
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="ModelException"></exception>
        private static void Main(string[] args)
        {
            Console.WriteLine("Building Sistema ..");
            ISistema sistema = Startup.BuildSistema();

            Console.WriteLine("Creating Persona ..");
            {
                Persona persona = new Persona()
                {
                    Rut = "130144918",
                    Nombre = "Diego",
                    Paterno = "Urrutia",
                    Materno = "Astorga",
                    Email = "durrutia@ucn.cl"
                };
                

                Console.WriteLine(persona);
                Console.WriteLine(Utils.ToJson(persona));

                // Save in the repository
                sistema.Save(persona);
            }

            Console.WriteLine("Finding personas ..");
            {
                IList<Persona> personas = sistema.GetPersonas();
                Console.WriteLine("Size: " + personas.Count);

                foreach (Persona persona in personas)
                {
                    Console.WriteLine("Persona = " + Utils.ToJson(persona));
                }
            }

            Console.WriteLine("Done.");
            
            Console.WriteLine("Creando cotizacion ");

            
            Cotizacion cotizacion = new Cotizacion();
            {
                DateTime hoy = DateTime.Now;
                string hoySTR = hoy.ToString();
                cotizacion.Id = 1;
                cotizacion.FechaCreacion = hoy;
                cotizacion.RutCliente = "174920524";
                cotizacion.RutUsuarioCreador = "147112912";
            }

            
            
            Console.WriteLine(cotizacion);
            Console.WriteLine(Utils.ToJson(cotizacion));
            //problema que el item sigue siendo null     
            sistema.Save(cotizacion);
            
        }
    }
}