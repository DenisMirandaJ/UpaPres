using System;
using System.Collections.Generic;
using Core.DAO;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace TestCore.DAO
{
    /// <summary>
    /// Testing del repositorio de cotizaciones
    /// </summary>
    public sealed class CotizacionRepositoryTests
    {
        /// <summary>
        /// Logger de la clase
        /// </summary>
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output"></param>
        public CotizacionRepositoryTests(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException("Se requiere la consola");
        }

        /// <summary>
        /// Test de insercion y busqueda en el repositorio.
        /// </summary>
        [Fact]
        public void InsercionBusquedaCotizacionTest()
        {
            // Contexto
            DbContext dbContext = BuildTestModelContext();

            // Repositorio de Cotizacions
            CotizacionRepository repo = new CotizacionRepository(dbContext);

            // Creacion
            {
                Cotizacion cotizacion = new Cotizacion();
                {
                    DateTime hoy = DateTime.Now;
                    string hoySTR = hoy.ToString();
                    cotizacion.Id = 1;
                    cotizacion.FechaCreacion = hoy;
                    cotizacion.RutCliente = "174920524";
                    cotizacion.RutUsuarioCreador = "147112912";
                }

                
                // Insert into the backend
                repo.Add(cotizacion);
                
            }
            
            // Busqueda (exitosa)
            {
                Cotizacion cotizacion = repo.GetById(1);
                Assert.NotNull(cotizacion);
            }
            
            // Busqueda (no exitosa)
            {
                Cotizacion cotizacion = repo.GetById(-1);
                Assert.Null(cotizacion);
            }
            
            // Todos
            {
                IList<Cotizacion> cotizaciones = repo.GetAll();
                Assert.NotEmpty(cotizaciones);
            }
            
            // Busqueda por id (exito)
            {
                IList<Cotizacion> cotizaciones = repo.GetAll(c => c.Id.Equals(1));
                Assert.NotEmpty(cotizaciones);
            }

            // Busqueda por id (no exito)
            {
                IList<Cotizacion> cotizaciones = repo.GetAll(c => c.Id.Equals(2));
                Assert.Empty(cotizaciones);
            }
            
            // Busqueda por rutCliente
            {
                Assert.NotEmpty(repo.GetAll(c => c.RutCliente.Equals("174920524")));
            }
            
            // Busqueda por rutUsuarioCreador
            {
                Assert.NotNull(repo.GetAll(c => c.RutUsuarioCreador.Equals("174920524")));
            }
            
            //busqueda por fecha (exito)
            {
                DateTime d1 = DateTime.ParseExact("01/01/2017", "dd/mm/yyyy", null);
                DateTime d2 = DateTime.ParseExact("01/01/2019", "dd/mm/yyyy", null);
                Assert.NotNull(repo.GetbyDate(d1, d2));
                Assert.NotEmpty(repo.GetbyDate(d1, d2));
            }
            
            //busqueda_por_fecha (no exito)
            {
                DateTime d1 = DateTime.ParseExact("01/01/2017", "dd/mm/yyyy", null);
                DateTime d2 = DateTime.ParseExact("02/01/2017", "dd/mm/yyyy", null);
                Assert.Empty(repo.GetbyDate(d1, d2));
            }
            
            //Eliminacion (no exito)

            {
                Cotizacion Cotizacion1 = repo.GetById(1);
                Assert.NotNull(Cotizacion1);
                Cotizacion1.Id = -1;
                
                repo.Remove(Cotizacion1);   
            }
            
            // Eliminacion
            {
                Cotizacion Cotizacion = repo.GetById(1);
                Assert.NotNull(Cotizacion);
                
                repo.Remove(Cotizacion);                
            }

        }

        /// <summary>
        /// Construccion del DbContext de prueba
        /// </summary>
        /// <returns></returns>
        private static DbContext BuildTestModelContext()
        {
            DbContextOptions<ModelDbContext> options = new DbContextOptionsBuilder<ModelDbContext>()
                // .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseSqlite(@"Data Source=Cotizacions.db") // SQLite
                .EnableSensitiveDataLogging()
                .Options;
            
            return new ModelDbContext(options);
        }
    }
}