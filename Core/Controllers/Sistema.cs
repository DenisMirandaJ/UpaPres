using System;
using System.Collections.Generic;
using System.Linq;
using Core.DAO;
using Core.Models;
using System.Collections;
using System.Data;

namespace Core.Controllers
{
    /// <summary>
    /// Implementacion de la interface ISistema.
    /// </summary>
    public sealed class Sistema : ISistema
    {
        // Patron Repositorio, generalizado via Generics
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/
        private readonly IPersonaRepository _repositoryPersona;

        private readonly IRepository<Usuario> _repositoryUsuario;

        private readonly ICotizacionRepository _repositoryCotizacion;

        private readonly IRepository<Item> _repositoryItem;

        /// <summary>
        /// Inicializa los repositorios internos de la clase.
        /// </summary>
        public Sistema(IPersonaRepository repositoryPersona, IRepository<Usuario> repositoryUsuario,
                        ICotizacionRepository repositoryCotizacion, IRepository<Item> RepositoryItem)
        {
            // Setter!
            _repositoryPersona = repositoryPersona ??
                                 throw new ArgumentNullException("Se requiere el repositorio de personas");
            _repositoryUsuario = repositoryUsuario ??
                                 throw new ArgumentNullException("Se requiere repositorio de usuarios");
            _repositoryCotizacion = repositoryCotizacion ??
                                 throw new ArgumentNullException("Se requiere base de datos de cotizaciones");
            _repositoryItem = RepositoryItem ??
                                 throw new ArgumentNullException("Se requiere base de datos de items");

            // Inicializacion del repositorio.
            _repositoryPersona.Initialize();
            _repositoryUsuario.Initialize();
            _repositoryCotizacion.Initialize();
            _repositoryItem.Initialize();
        }

        /// <inheritdoc />
        public void Save(Persona persona)
        {
            // Verificacion de nulidad
            if (persona == null)
            {
                throw new ModelException("Persona es null");
            }

            // Saving the Persona en el repositorio.
            // La validacion de los atributos ocurre en el repositorio.
            _repositoryPersona.Add(persona);
        }

        /// <inheritdoc />
        public IList<Persona> GetPersonas()
        {
            return _repositoryPersona.GetAll();
        }

        /// <inheritdoc />
        public void Save(Persona persona, string password)
        {
            // Guardo o actualizo en el backend.
            _repositoryPersona.Add(persona);

            // Busco si el usuario ya existe
            Usuario usuario = _repositoryUsuario.GetAll(u => u.Persona.Equals(persona)).FirstOrDefault();
            
            // Si no existe, lo creo
            if (usuario == null)
            {
                usuario = new Usuario()
                {
                    Persona =  persona
                };
            }
            
            // Hash del password
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(password);
            
            // Almaceno en el backend
            _repositoryUsuario.Add(usuario);
            
        }

        public void Save(Cotizacion cotizacion)
        {    
            
            // ver si la cotizacion existe 
            
            //Cotizacion coti = _repositoryCotizacion.GetAll(u => u.Cotizacion.Equals(cotizacion)).FirstOrDefault();
            
            
            if (cotizacion == null)
            {
                throw new ModelException("La cotizacion no puede ser nula");
            }

            _repositoryCotizacion.Add(cotizacion);
        }

        /// <inheritdoc />
        public Usuario Login(string rutEmail, string password)
        {
            Persona persona = _repositoryPersona.GetByRutOrEmail(rutEmail);
            if (persona == null)
            {
                throw new ModelException("Usuario no encontrado");
            }
            
            IList<Usuario> usuarios = _repositoryUsuario.GetAll(u => u.Persona.Equals(persona));
            if (usuarios.Count == 0)
            {
                throw new ModelException("Existe la Persona pero no tiene credenciales de acceso");
            }

            if (usuarios.Count > 1)
            {
                throw new ModelException("Mas de un usuario encontrado");
            }

            Usuario usuario = usuarios.Single();
            if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password))
            {
                throw new ModelException("Password no coincide");
            }

            return usuario;

        }

        /// <inheritdoc />
        public Persona Find(string rutEmail)
        {
            return _repositoryPersona.GetByRutOrEmail(rutEmail);
        }


        /// <summary>
        /// Retorna el tipo de busqueda segun el criterio de busqueda
        /// Los tipos de busqueda posibles son:
        /// Rut: Para buscar cotizaciones segun el rut del Usuario creador o el rut del cliente
        /// Texto: Para coincidencias de texto dentro de los items de las cotizaciones
        /// Fecha: buscar una cotizacion creada en la fecha ingresada
        /// </summary>
        /// <param name="criterioBusqueda"></param>
        /// <returns>String con el tipo de busqueda a realizar</returns>
        public String TipoBusqueda(String criterioBusqueda)
        {
            if (String.IsNullOrEmpty(criterioBusqueda) || String.IsNullOrWhiteSpace(criterioBusqueda))
            {
                throw new ArgumentException("El criterio de busqueda no puede estar vacio");
            }
            try
            {
                //check si el criterio de busqueda es un rut
                Models.Validate.ValidarRut(criterioBusqueda);
                return "Rut";
            }
            catch (ModelException e) { ; }

            DateTime t;
            if (DateTime.TryParse(criterioBusqueda, out t))
            {
                return "Fecha";
            }

            return "Texto";
        }

        /// <summary>
        /// Busca una cotizacion, identificando el criterio de busqueda
        /// </summary>
        /// <param name="criterioDeBusqueda"></param>
        /// <returns>Lista de cotizaciones que cumplan con el criterio de busqueda</returns>
        public IList<Cotizacion> BuscarCotizacion(String criterioDeBusqueda)
        {
            //metodo local para buscar por coincidencias de texto dentro de las cotizaciones
            IList<Cotizacion> BuscarPorTexto(String _criterioDeBusqueda)
            {
                IList<Cotizacion> cotizaciones= _repositoryCotizacion.GetAll();
                HashSet<Cotizacion> cotizacionesElegidas =  new HashSet<Cotizacion>(); //no queremos repeticiones, en caso de haberlas
                foreach (Cotizacion cotizacion in cotizaciones)
                {
                    foreach (Item items in cotizacion.Items)
                    {
                        if (items.descripcion.Contains(_criterioDeBusqueda))
                        {
                            cotizacionesElegidas.Add(cotizacion);
                        }
                    }
                }

                return cotizacionesElegidas.ToList();
            }
            
            try
            {
                TipoBusqueda(criterioDeBusqueda); //asegurandonos que el criterio de busqueda no sea null o este vacio
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("El criterio de busqueda no puede estar vacio");
                throw e;
            }

            String tipoBusqueda = TipoBusqueda(criterioDeBusqueda);
            switch (tipoBusqueda) 
            {
                case  "Rut":
                    IList<Cotizacion> cot = _repositoryCotizacion.GetAll(c => c.RutCliente.Equals(criterioDeBusqueda) || 
                                                             c.RutUsuarioCreador.Equals(criterioDeBusqueda));
                    return cot;
                case "Texto":
                    return BuscarPorTexto(criterioDeBusqueda); 
                case "Fecha":
                    return _repositoryCotizacion.GetAll(
                        c => c.FechaCreacion.Equals(DateTime.ParseExact(criterioDeBusqueda, "dd/mm/yyyy", null)));
            }

            return null; //nunca se deberia llegar aqui /s
        }

        /// <summary>
        /// Encuentra una cotizacion segun el criterio de busqueda
        /// Ademas filtra los resultados por fecha
        /// </summary>
        /// <param name="criterioBusqueda"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaTermino"></param>
        /// <returns>Cotizaciones encontrdaas filtradas por fecha</returns>
        /// <exception cref="ArgumentException"></exception>
        public IList<Cotizacion> BuscarCotizacionEntreFechas(String criterioBusqueda, DateTime fechaInicio,
            DateTime fechaTermino)
        {
            if (fechaInicio == null || fechaTermino == null)
            {
                throw new ArgumentNullException("Las fechas de busqueda no pueden ser null");
            }
            try
            {
                HashSet<Cotizacion> cotizaciones = new HashSet<Cotizacion>(); //Set para no tener duplicados en caso de haberlos
                IList<Cotizacion> cotizacionesEncontradas= BuscarCotizacion(criterioBusqueda);
                foreach (Cotizacion cotizacion in cotizacionesEncontradas)
                {
                    if (cotizacion.FechaCreacion >= fechaInicio && cotizacion.FechaCreacion <= fechaTermino)
                    {
                        cotizaciones.Add(cotizacion);
                    }
                }

                return cotizaciones.ToList();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                throw new ArgumentException("El criterio de busqueda no puede estar vacio o ser nulo");
            }
        }
    }
}