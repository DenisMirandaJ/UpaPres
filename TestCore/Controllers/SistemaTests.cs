using System;
using Core;
using Core.Controllers;
using Core.DAO;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace TestCore.Controllers
{    
    
    /// <summary>
    /// Test del sistema
    /// </summary>
    public class SistemaTests
    {
        
        /// <summary>
        /// Logger de la clase
        /// </summary>
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output"></param>
        public SistemaTests(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }


        [Fact]
        public void SistemaConstructorTest()
        {
            DbContextOptions<ModelDbContext> options = new DbContextOptionsBuilder<ModelDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            DbContext dbContext = new ModelDbContext(options);
            IPersonaRepository personas = new PersonaRepository(dbContext);
            IRepository<Usuario> usuarios = new ModelRepository<Usuario>(dbContext);
            ICotizacionRepository cotizaciones = new CotizacionRepository(dbContext);
            IRepository<Item> items = null;

            //Cuando faltan repositorios
            Assert.Throws<ArgumentNullException>(() => new Sistema(personas, usuarios, cotizaciones, items));

        }
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void LoginTest()
        {
            _output.WriteLine("Starting Sistema test ...");
            ISistema sistema = Startup.BuildSistema();
            
            // Insert persona
            {
                _output.WriteLine("Testing insert ..");
                Persona persona = new Persona()
                {
                    Rut = "130144918",
                    Nombre = "Diego",
                    Paterno = "Urrutia",
                    Materno = "Astorga",
                    Email = "durrutia@ucn.cl"
                };

                sistema.PersonaSave(persona);
            }
            
            // GetPersonas
            {
                _output.WriteLine("Testing getPersonas ..");
                Assert.NotEmpty(sistema.GetPersonas());
            }
            
            // Buscar persona
            {
                _output.WriteLine("Testing Find ..");
                Assert.NotNull(sistema.Find("durrutia@ucn.cl"));
                Assert.NotNull(sistema.Find("130144918"));
            }
            
            // Busqueda de usuario
            {
                Exception usuarioNoExiste =
                    Assert.Throws<ModelException>(() => sistema.Login("notfound@ucn.cl", "durrutia123"));
                Assert.Equal("Usuario no encontrado", usuarioNoExiste.Message);
                
                Exception usuarioNoExistePersonaSi =
                    Assert.Throws<ModelException>(() => sistema.Login("durrutia@ucn.cl", "durrutia123"));
                Assert.Equal("Existe la Persona pero no tiene credenciales de acceso", usuarioNoExistePersonaSi.Message);                
            }
            
            // Insertar usuario
            {
                Persona persona = sistema.Find("durrutia@ucn.cl");
                Assert.NotNull(persona);
                _output.WriteLine("Persona: {0}", Utils.ToJson(persona));
                
                sistema.UsuarioSave(persona, "durrutia123");
            }

            // Busqueda de usuario
            {
                Exception usuarioExisteWrongPassword =
                    Assert.Throws<ModelException>(() => sistema.Login("durrutia@ucn.cl", "este no es mi password"));
                Assert.Equal("Password no coincide", usuarioExisteWrongPassword.Message);

                Usuario usuario = sistema.Login("durrutia@ucn.cl", "durrutia123");
                Assert.NotNull(usuario);
                _output.WriteLine("Usuario: {0}", Utils.ToJson(usuario));

            }

        }

        [Fact]
        public void BusquedaDeCotizacionesTest()
        {
            _output.WriteLine("Starting Sistema test ...");
            ISistema sistema = Startup.BuildSistema();
            
            //Insert Cotizaciones
            {
                _output.WriteLine("Testing insert ..");
                Cotizacion cotizacion = new Cotizacion();
                {
                    cotizacion.Id = 1;
                    cotizacion.FechaCreacion = DateTime.Now;
                    cotizacion.RutCliente = "174920524";
                    cotizacion.RutUsuarioCreador = "147112912";
                    cotizacion.Items = new List<Item>();
                };

                Item item1 = new Item();
                {
                    item1.descripcion = "item de prueba1";
                    item1.precio = 250000;
                };

                Item item2 = new Item();
                {
                    item2.descripcion = "item de prueba2";
                    item2.precio = 200000;
                }

                Console.WriteLine(item1.descripcion);
                cotizacion.Items.Add(item1);
                cotizacion.Items.Add(item2);

                Console.WriteLine(cotizacion);
                Console.WriteLine(Utils.ToJson(cotizacion));
                //problema que el item sigue siendo null     
                sistema.AgregarCotizacion(cotizacion);
            }
            _output.WriteLine("Done..");
            _output.WriteLine("Probando criterio de busqueda null o vacio");
            //Probar criterio de busqueda en blanco o null
            {
                Assert.Throws<ArgumentException>(() => sistema.TipoBusqueda(" "));
                Assert.Throws<ArgumentException>(() => sistema.TipoBusqueda(""));
                Assert.Throws<ArgumentException>(() => sistema.TipoBusqueda(null));
                Assert.Throws<ArgumentException>(() => sistema.BuscarCotizacion(" "));
                Assert.Throws<ArgumentException>(() => sistema.BuscarCotizacion(""));
                Assert.Throws<ArgumentException>(() => sistema.BuscarCotizacion(null));
            }
            _output.WriteLine("Done..");

            _output.WriteLine("Probando busqueda por rut..");
            //Probar busqueda por rut (exito)
            {
                //buscar por rut del UsuarioCreador
                IList<Cotizacion> cotizaciones1 = sistema.BuscarCotizacion("174920524");
                //buscar por rut del cliente
                IList<Cotizacion> cotizaciones2 = sistema.BuscarCotizacion("147112912");
                Assert.NotNull(cotizaciones1);
                Assert.NotNull(cotizaciones2);
                Assert.NotEmpty(cotizaciones1);
                Assert.NotEmpty(cotizaciones2);
            }

            //probar busqueda por rut (no exito)
            {
                //buscar por rut
                IList<Cotizacion> cotizaciones1 = sistema.BuscarCotizacion("168174209");
                Assert.Empty(cotizaciones1);
            }
            _output.WriteLine("Done..");
            _output.WriteLine("Probando busqueda por texto");
            //buscar por texto (exito)
            {
                //La cotizaciones de prueba contiene "item de prueba1" en su descripcion
                IList<Cotizacion> cotizaciones = sistema.BuscarCotizacion("prueba1");
                Assert.NotEmpty(cotizaciones);
            }
            
            //buscar por texto (no exito)
            {
                IList<Cotizacion> cotizaciones = sistema.BuscarCotizacion("exto no existe en las cotizaciones");
                Assert.Empty(cotizaciones);
            }
            _output.WriteLine("Done..");
            
            //busqueda por fecha (exito)
            _output.WriteLine("probando busqueda por fecha");
            {
                DateTime t0 = DateTime.ParseExact("01/01/2018", "dd/mm/yyyy",null);
                DateTime tf = DateTime.ParseExact("01/01/2019", "dd/mm/yyyy",null);
                IList<Cotizacion> cotizaciones = sistema.BuscarCotizacionEntreFechas("prueba1",t0,tf);
                Assert.NotEmpty(cotizaciones);
            }
            //busqueda por fecha (no exito)
            {
                DateTime t0 = DateTime.ParseExact("01/01/2017", "dd/mm/yyyy",null);
                DateTime tf = DateTime.ParseExact("01/01/2018", "dd/mm/yyyy",null);
                IList<Cotizacion> cotizaciones = sistema.BuscarCotizacionEntreFechas("prueba1",t0,tf);
                Assert.Empty(cotizaciones);
            }
            _output.WriteLine("Done..");
            
            //Esto no fue necesario ya que DateTime se inicializa automaticamente en DateTime.MinValue
            //https://stackoverflow.com/questions/221732/datetime-null-value
            //Probar fechas null
            //{
            //    DateTime t = null;
            //    Assert.Throws<ArgumentNullException>(() => sistema.BuscarCotizacionEntreFechas("criterio", t, t));
            //}
        }

        [Fact]
        public void EliminarCotizacionTest()
        {
            _output.WriteLine("Starting Sistema test ...");
            ISistema sistema = Startup.BuildSistema();
            {
                _output.WriteLine("Testing insert ..");
                Cotizacion cotizacion = new Cotizacion();
                {
                    cotizacion.Id = 1;
                    cotizacion.FechaCreacion = DateTime.Now;
                    cotizacion.RutCliente = "174920524";
                    cotizacion.RutUsuarioCreador = "147112912";
                    cotizacion.Items = new List<Item>();
                };

                Item item1 = new Item();
                {
                    item1.descripcion = "item de prueba1";
                    item1.precio = 250000;
                };

                Item item2 = new Item();
                {
                    item2.descripcion = "item de prueba2";
                    item2.precio = 200000;
                }

                Console.WriteLine(item1.descripcion);
                cotizacion.Items.Add(item1);
                cotizacion.Items.Add(item2);

                Console.WriteLine(cotizacion);
                Console.WriteLine(Utils.ToJson(cotizacion));
                //problema que el item sigue siendo null     
                sistema.AgregarCotizacion(cotizacion);
            }
            _output.WriteLine("Done..");
            _output.WriteLine("Probando criterio de id de eliminacion");
            
            //Probar criterio de busqueda en blanco o null
            //Probar criterio de eliminacion inxistente 
            {
                Assert.Throws<ArgumentException>(() => sistema.EliminarCotizacion(91823719)); //numero inexistente
                Assert.Throws<ArgumentException>(() => sistema.EliminarCotizacion(0)); // numero 0
                Assert.Throws<ArgumentException>(() => sistema.BuscarCotizacion(" "));
                Assert.Throws<ArgumentException>(() => sistema.BuscarCotizacion(""));
                Assert.Throws<ArgumentException>(() => sistema.BuscarCotizacion(null));
            }
            _output.WriteLine("Done..");
            
            
            
            
        }

        [Fact]
        public void AgregarCotizacionTest()
        {

            _output.WriteLine("Starting Sistema test ...");
            ISistema sistema = Startup.BuildSistema();
            Cotizacion cotizacionCorrecta = new Cotizacion()
            {
                Id = 194953607,
                RutCliente = "194953607",
                FechaCreacion = DateTime.Now,
                RutUsuarioCreador = "191234567",
                Items = new List<Item>()
            };

            Item item1 = new Item();
            {
                item1.descripcion = "item de prueba1";
                item1.precio = 250000;
            }

            Item item2 = new Item();
            {
                item2.descripcion = "item de prueba2";
                item2.precio = 200000;
            }



            _output.WriteLine("Se ha creado una cotizacion correcta ");

            int idINC = 123454;
            string rutCliINC = "1";
            DateTime fechaCreINC = DateTime.Parse("123817");
            string rutUsiCreINC = "10293109";

            // Models.ValidateTests.ValidateRutCorrecto(rutCliINC);

            if (idINC != Int32.Parse(rutCliINC))
            {
                _output.WriteLine("La cotizacion con el rut no tienen coincidencia con la id");
            }

            if (fechaCreINC != DateTime.Today)
            {
                _output.WriteLine("La cotizacion en su fecha de creacion no es la correcta");
            }

            new Models.ValidateTests().ValidateRutCorrecto(rutCliINC);
            new Models.ValidateTests().ValidateRutCorrecto(rutUsiCreINC);
            // si estas 2 validaciones se comprueban se crearian las cotizaciones 




        



         }

        [Fact]
        public void EditarCotizacionTest()
        {
            
            _output.WriteLine("Starting Sistema test ...");
            ISistema sistema = Startup.BuildSistema();
            Cotizacion cotizacionCorrecta= new Cotizacion()
            // edicion Correcta 
            {
                Id = 194953607,
                RutCliente = "194953607",
                FechaCreacion = DateTime.Now,
                RutUsuarioCreador = "191234567",
                Items = new List<Item>()
            };
                
            Item item1 = new Item();
            {
                item1.descripcion = "item de prueba1";
                item1.precio = 250000;
            }

            Item item2 = new Item();
            {
                item2.descripcion = "item de prueba2";
                item2.precio = 200000;
            }

            
            
            _output.WriteLine("Se ha creado una cotizacion correcta ");
            string rutCambio = "8932897";
            DateTime fechaCambio = DateTime.Parse("123817");
            _output.WriteLine(" Comprobando su Rut ... ");
            new Models.ValidateTests().ValidateRutCorrecto(rutCambio);
            if (fechaCambio >= DateTime.Now)
            {
                _output.WriteLine("Imposible realizar esta cambio con la fecha Mayor al dia actual");
            }
            
            //no deberia aceptar una id 0
            Assert.Throws<ArgumentNullException>(() => sistema.EditarCotizacion(0, "rut", "cambio123"));
            //no deberia aceptar un campo null
            Assert.Throws<ArgumentNullException>(() => sistema.EditarCotizacion(1, null, "cambio123"));
            // no deberia acpetar un Cambio vacio
            Assert.Throws<ArgumentNullException>(() => sistema.EditarCotizacion(1, "rut", ""));


        }


        [Fact]
        public void BuscarUsrTest()
        {
            _output.WriteLine("Starting Sistema test ...");
            ISistema sistema = Startup.BuildSistema();
            Cotizacion cotizacionCorrecta= new Cotizacion()
                // edicion Correcta 
                {
                    Id = 194953607,
                    RutCliente = "194953607",
                    FechaCreacion = DateTime.Now,
                    RutUsuarioCreador = "191234567",
                    Items = new List<Item>()
                };
                
            Item item1 = new Item();
            {
                item1.descripcion = "item de prueba1";
                item1.precio = 250000;
            }

            Item item2 = new Item();
            {
                item2.descripcion = "item de prueba2";
                item2.precio = 200000;
            }
            
            //no deberia aceptar un criterio 0 ya que nada deberia tener cero en su comienzo
            Assert.Throws<ArgumentNullException>(() => sistema.BuscarCotizacion("0"));
            //no deberia aceptar null
            Assert.Throws<ArgumentNullException>(() => sistema.BuscarCotizacion(null));
        }

        [Fact]
        public void PersonaSaveTest()
        {
            _output.WriteLine("Starting PErsonaSAveTEst ...");
            ISistema sistema = Startup.BuildSistema();
            Persona personaCorrecto = new Persona()
            {
                Rut = "130144918",
                Nombre = "Diego",
                Paterno = "Urrutia",
                Materno = "Astorga",
                Email = "durrutia@ucn.cl"
            };
            
            Persona personaIncorrecto = new Persona()
            {
                Rut = "",
                Nombre = "Diego",
                Paterno = "Urrutia",
                Materno = "Astorga",
                Email = "durrutia@ucn.cl"
            };
            
            //Psibles errores
            Assert.Throws<ModelException>(() => sistema.PersonaSave(null));
            Assert.Throws<ModelException>(() => sistema.PersonaSave(personaIncorrecto));
            _output.WriteLine("PersonaSaveTest Done ...");

        }

        [Fact]
        public void GetPersonasTest()
        {
            _output.WriteLine("Starting GetPersonasTest ...");
            ISistema sistema = Startup.BuildSistema();
            Persona personaCorrecto = new Persona()
            {
                Rut = "130144918",
                Nombre = "Diego",
                Paterno = "Urrutia",
                Materno = "Astorga",
                Email = "durrutia@ucn.cl"
            };
            
            //No exito
            Assert.Empty(sistema.GetPersonas());
            sistema.PersonaSave(personaCorrecto);
            //Exito
            Assert.NotEmpty(sistema.GetPersonas());
        }

        [Fact]
        public void UsuarioSaveTest()
        {
            _output.WriteLine("Starting UsuarioSaveTest ...");
            ISistema sistema = Startup.BuildSistema();
            Persona personaCorrecto = new Persona()
            {
                Rut = "130144918",
                Nombre = "Diego",
                Paterno = "Urrutia",
                Materno = "Astorga",
                Email = "durrutia@ucn.cl"
            };
            Persona personaIncorrecto = new Persona()
            {
                Rut = "", //rut vacio
                Nombre = "Diego",
                Paterno = "Urrutia",
                Materno = "Astorga",
                Email = "durrutia@ucn.cl"
            };

            //no deberia aceptar persona nula
            Assert.Throws<ArgumentNullException>(() => sistema.UsuarioSave(null, "password"));
            //Ni password nulas o vacias
            Assert.Throws<ArgumentNullException>(() => sistema.UsuarioSave(personaCorrecto, null));
            Assert.Throws<ArgumentNullException>(() => sistema.UsuarioSave(personaCorrecto, ""));
            //Persona con formato incorrecto
            Assert.Throws<ModelException>(() => sistema.UsuarioSave(personaIncorrecto, null));
            
            //agregar usuario nuevo (exito)
            sistema.UsuarioSave(personaCorrecto, "1234");
            //actualizat usuario (extio)
            sistema.UsuarioSave(personaCorrecto,"4321");

        }
    }
}