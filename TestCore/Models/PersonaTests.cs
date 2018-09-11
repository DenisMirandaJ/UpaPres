using System;
using Core.Models;
using Core;
using Xunit;

namespace TestCore.Models
{
    /// <summary>
    /// Testing de la clase Persona.
    /// </summary>
    public class PersonaTests
    {
        /// <summary>
        /// Test del constructor
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            Console.WriteLine("Creating Persona ..");
            Persona persona = new Persona()
            {
                Rut = "13014491-8",
                Nombre = "Diego",
                Paterno = "Urrutia",
                Materno = "Astorga"
            };

            Console.WriteLine(persona);
        }

        [Fact]
        public void ValidateTest()
        {
            Persona persona = new Persona()
            {
                Rut = "13014491-8",
                Nombre = "Diego",
                Paterno = "Urrutia",
                Materno = "Astorga"
            };
            
            //test rut null o blanco
            persona.Rut = null;
            Assert.Throws<ModelException>(() => persona.Validate());
            persona.Rut = "13014491-8";

            persona.Nombre = "";
            Assert.Throws<ModelException>(() => persona.Validate());
            persona.Nombre = "Diego";

        }
    }
}