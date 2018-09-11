using System;
using System.Collections.Generic;
using Core;
using Core.Models;
using Xunit;

namespace TestCore.Models
{
    /// <summary>
    /// Testing de la clase Cotizacion.
    /// </summary>
    public class CotizacionTests
    {
        /// <summary>
        /// Test del constructor
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            Console.WriteLine("Creating Cotizacion ..");
            Cotizacion cotizacion = new Cotizacion()
            {
                Id = 1,
                RutCliente = "147112912",
                RutUsuarioCreador = "174920524",
                FechaCreacion = DateTime.Now,
                Items = new List<Item>()
                
            };

            Console.WriteLine(cotizacion);
        }

        [Fact]
        public void ValidateTest()
        {
            Cotizacion cotizacion = new Cotizacion()
            {
                Id = 1,
                RutCliente = "147112912",
                RutUsuarioCreador = "174920524",
                FechaCreacion = DateTime.MaxValue,
                Items = new List<Item>()
                
            };
            //fecha de cracion en el futuro
            Assert.Throws<ModelException>(() => cotizacion.Validate());

        }
    }
}