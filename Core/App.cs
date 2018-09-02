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
                cotizacion.Id = 1;
                cotizacion.fecha_creacion = "31-08-2018";
                cotizacion.rut_cliente = "19.495.360-7";
                cotizacion.rut_Usuario_Creador = "14.711.291-2";
                
            }

            Item item1 = new Item();
            {
                item1.CotizacionId = cotizacion.Id;
                item1.descripcion = "trabajo numero 1";
                item1.precio = 200000;
            }
            
            Item item2 = new Item();
            {
                item2.CotizacionId = cotizacion.Id;
                item2.descripcion = "trabajo numero 2";
                item2.precio = 500000;
            }
            
            Console.WriteLine(cotizacion);
            Console.WriteLine(Utils.ToJson(cotizacion));
            sistema.Save(cotizacion);
            
        }
    }
}