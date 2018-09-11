
using System.Collections.Generic;
using Core.Models;
using System;

namespace Core.Controllers
{
    /// <summary>
    /// Operaciones del sistema.
    /// </summary>
    public interface ISistema
    {
        /// <summary>
        /// Operacion de sistema: Almacena una persona en el sistema.
        /// </summary>
        /// <param name="persona">Persona a guardar en el sistema.</param>
        void PersonaSave(Persona persona);

        void BuscarUsuario(string busqueda);

        /// <summary>
        /// Obtiene todas las personas del sistema.
        /// </summary>
        /// <returns>The IList of Persona</returns>
        IList<Persona> GetPersonas();

        /// <summary>
        /// Guarda a un usuario en el sistema
        /// </summary>
        /// <param name="persona"></param>
        /// <param name="password"></param>
        void UsuarioSave(Persona persona, string password);

        void EliminarCotizacion(int id);
        /// <summary>
        /// Guarda una cotizacion en el sistema
        /// </summary>

        void AgregarCotizacion(Cotizacion cotizacion);

        void EditarCotizacion<T>(int id , string campo , T cambio);
        
        

        
        

        /// <summary>
        /// Obtiene el usuario desde la base de datos, verificando su login y password.
        /// </summary>
        /// <param name="rutEmail">RUT o Correo Electronico</param>
        /// <param name="password">Contrasenia de acceso al sistema</param>
        /// <returns></returns>
        Usuario Login(string rutEmail, string password);

        /// <summary>
        /// Busqueda de una persona por rut o correo electronico.
        /// </summary>
        /// <param name="rutEmail">RUT o Correo Electronico</param>
        /// <returns>La persona si existe</returns>
        Persona Find(string rutEmail);

        /// <summary>
        /// busca cotizaciones segun el criterio de busqueda, la cotizacion debe estar entre dos fechas
        /// </summary>
        /// <param name="criteriobusqueda"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaTermino"></param>
        /// <returns>Una lista de cotizaciones que cumplan con el criterio de busqueda</returns>
        IList<Cotizacion> BuscarCotizacion(String criterioDebusqueda);

        IList<Cotizacion> BuscarCotizacionEntreFechas(String criterioBusqueda, DateTime fechaInicio,
            DateTime fechaTermino);

        //Cotizacion BuscarCotizacion(String criteriobusqueda, DateTime fechaInicio, DateTime fechaTermino);

        /// <summary>
        /// define el tipo de busqueda necesario
        /// </summary>
        /// <param name="criterioBusqueda"></param>
        /// <returns></returns>
        String TipoBusqueda(String criterioBusqueda);

        //String EliminarCotizacion(int id);
        
    }
}