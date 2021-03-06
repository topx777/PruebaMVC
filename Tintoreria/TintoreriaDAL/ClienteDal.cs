﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Upds.Sistemas.ProgWeb2.Tintoreria.Core;

namespace Upds.Sistemas.ProgWeb2.Tintoreria.TintoreriaDAL
{
    public class ClienteDal
    {
        /// <summary>
        /// Inserta nuevo Cliente a la base  de datos
        /// </summary>
        /// <param name="cliente"></param>
        public static void Insert(Cliente cliente)
        {
            Methods.GenerateLogsDebug("ClienteDal", "Insertar", string.Format("{0} Info: {1}", DateTime.Now.ToLongDateString(), "Empezando a ejecutar el metodo acceso a datos para Insertar un paciente"));

            //List<SqlCommand> commands = new List<SqlCommand>();
            SqlCommand command = null;

            // Proporcionar la cadena de consulta 
            string queryString = @"INSERT INTO Cliente(IdPersona, Nit, Razon, FechaRegistro)
                                    VALUES(@idPersona, @nit, @razon, @fechaRagistro)";
            try
            {
                //Registro Usuario
                UsuarioDal.Insertar(cliente.Usuario);
                cliente.Usuario.IdUsuario = Methods.GetActIDTable("Usuario");

                //Registro Persona
                Persona persona = cliente;
                PersonaDal.Insertar(persona);
                cliente.IdPersona = Methods.GetActIDTable("Persona");

                //Registro Cliente
                command = Methods.CreateBasicCommand(queryString);
                command.Parameters.AddWithValue("@idPersona",cliente.IdPersona);
                command.Parameters.AddWithValue("@nit", cliente.Nit);
                command.Parameters.AddWithValue("@razon", cliente.Razon);
                command.Parameters.AddWithValue("@fechaRagistro", DateTime.Now);
                Methods.ExecuteBasicCommand(command);

                foreach (Telefono telefono in cliente.Telefonos)
                {
                    TelefonoDal.Insertar(telefono, cliente.IdPersona);
                }
                foreach (Direccion direccion in cliente.Direcciones)
                {
                    DireccionDal.Insertar(direccion, cliente.IdPersona);
                }
                foreach (Correo correo in cliente.Correos)
                {
                    CorreoDal.Insertar(correo, cliente.IdPersona);
                }

                //Methods

            }
            catch (SqlException ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "Insertar", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }
            catch (Exception ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "Insertar", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }

            Methods.GenerateLogsDebug("ClienteDal", "Insertar", string.Format("{0} {1} Info: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), "Termino de ejecutar  el metodo acceso a datos para insertar un Cliente"));

        }

        public static void Insertar(Cliente cliente)
        {
            Methods.GenerateLogsDebug("ClienteDal", "InsertarCliente", string.Format("{0} Info: {1}",
            DateTime.Now.ToLongDateString(), "Empezando a ejecutar el metodo acceso a datos para crear un Cliente"));

            SqlCommand command = null;
            SqlTransaction trans = null;

            //Consulta para insertar datos de Cliente
            string queryString = @"INSERT INTO Cliente(IdPersona, Nit, Razon, FechaRegistro)
                                    VALUES(@idPersona, @nit, @razon, @fechaRagistro)";

            SqlConnection conexion = Methods.ObtenerConexion();

            try
            {
                conexion.Open();
                SqlCommand usuarioInsertcmd = UsuarioDal.InsertarOUTPUT(cliente.Usuario);
                //Inicio de Conexion a la Base de Datos
                trans = conexion.BeginTransaction();

                usuarioInsertcmd.Connection = conexion;
                usuarioInsertcmd.Transaction = trans;
                cliente.Usuario.IdUsuario = Convert.ToInt32(usuarioInsertcmd.ExecuteScalar());

                Persona persona = cliente;

                SqlCommand personaInsertcmd = PersonaDal.InsertarOUTPUT(persona);

                personaInsertcmd.Connection = conexion;
                personaInsertcmd.Transaction = trans;
                cliente.IdPersona = Convert.ToInt32(personaInsertcmd.ExecuteScalar());


                // Creacion de Cliente Commando y ejecutado
                command = new SqlCommand(queryString);
                command.Parameters.AddWithValue("@idPersona", cliente.IdPersona);
                command.Parameters.AddWithValue("@nit", cliente.Nit);
                command.Parameters.AddWithValue("@razon", cliente.Razon);
                command.Parameters.AddWithValue("@fechaRagistro", DateTime.Now);

                command.Connection = conexion;
                command.Transaction = trans;
                command.ExecuteNonQuery();

                //Insertar telefonos
                foreach (Telefono telf in cliente.Telefonos)
                {
                    SqlCommand telfcmd = TelefonoDal.InsertarOUTPUT(telf, cliente.IdPersona);
                    telfcmd.Connection = conexion;
                    telfcmd.Transaction = trans;
                    telfcmd.ExecuteNonQuery();
                }

                //Insertar direcciones
                foreach (Direccion direc in cliente.Direcciones)
                {
                    SqlCommand direccmd = DireccionDal.InsertarOUTPUT(direc, cliente.IdPersona);
                    direccmd.Connection = conexion;
                    direccmd.Transaction = trans;
                    direccmd.ExecuteNonQuery();
                }

                //Insertar correos
                foreach (Correo correo in persona.Correos)
                {
                    SqlCommand correocmd = CorreoDal.InsertarOUTPUT(correo, cliente.IdPersona);
                    correocmd.Connection = conexion;
                    correocmd.Transaction = trans;
                    correocmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
            catch (SqlException ex)
            {
                trans.Rollback();
                Methods.GenerateLogsRelease("ClienteDal", "InsertarCliente", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                Methods.GenerateLogsRelease("ClienteDal", "InsertarCliente", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }
            finally
            {
                conexion.Close();
            }

            Methods.GenerateLogsDebug("ClienteDal", "InsertarCliente", string.Format("{0} {1} Info: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), "Termino de ejecutar  el metodo acceso a datos para insertar un Cliente"));
        }

        /// <summary>
        /// Elimina de forma logica un cliente de la base de datos
        /// </summary>
        /// <param name="id"></param>
        public static void Eliminar(int id)
        {
            Methods.GenerateLogsDebug("ClienteDal", "Eliminar", string.Format("{0} Info: {1}", DateTime.Now.ToLongDateString(), "Empezando a ejecutar el metodo acceso a datos para eliminar un Cliente"));

            SqlCommand command = null;

            // Proporcionar la cadena de consulta 
            string queryString = "UPDATE Persona SET Borrado = 1 WHERE IdPersona=@id";
            try
            {
                command = Methods.CreateBasicCommand(queryString);
                command.Parameters.AddWithValue("@id", id);
                Methods.ExecuteBasicCommand(command);
            }
            catch (SqlException ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "Eliminar", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }
            catch (Exception ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "Eliminar", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }

                Methods.GenerateLogsDebug("ClienteDal", "Eliminar", string.Format("{0} {1} Info: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), "Termino de ejecutar  el metodo acceso a datos para Eliminar un Cliente"));

        }

        /// <summary>
        /// Actualiza un Cliente de la base de datos
        /// </summary>
        /// <param name="cliente"></param>
        public static void Actualizar(Cliente cliente)  
        {
            Methods.GenerateLogsDebug("ClienteDal", "Actualizar", string.Format("{0} Info: {1}", DateTime.Now.ToLongDateString(), "Empezando a ejecutar el metodo acceso a datos para Actualizar un Cliente"));

            SqlCommand command = null;

            // Proporcionar la cadena de consulta 
            string queryString = @"UPDATE Cliente SET Nit=@nit, Razon=@razon
                                    WHERE @IdPersona=@idPersona";
            try
            {
                //Actualiza Usuario
                UsuarioDal.Actualizar(cliente.Usuario);

                //Actualiza Persona
                Persona persona = cliente;
                PersonaDal.Actualizar(persona);

                //Actualiza Cliente
                command = Methods.CreateBasicCommand(queryString);
                command.Parameters.AddWithValue("@nit", cliente.Nit);
                command.Parameters.AddWithValue("@razon", cliente.Razon);
                command.Parameters.AddWithValue("@idPersona", cliente.IdPersona);
                Methods.ExecuteBasicCommand(command);

                //Actualiza Correos
                foreach (Correo correo in cliente.Correos)
                {
                    CorreoDal.Actualizar(correo);
                }
                //Actualiza Telefonos
                foreach (Telefono telefono in cliente.Telefonos)
                {
                    TelefonoDal.Actualizar(telefono);
                }
                //Actuliza Direcciones
                foreach (Direccion direccion in cliente.Direcciones)
                {
                    DireccionDal.Actualizar(direccion);
                }

            }
            catch (SqlException ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "Actualizar", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }
            catch (Exception ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "Actualizar", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }

                Methods.GenerateLogsDebug("ClienteDal", "Insertar", string.Format("{0} {1} Info: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), "Termino de ejecutar  el metodo acceso a datos para Actualizar un paciente"));

        }

        
        /// <summary>
        /// Obtiene un Cliente de la base de datos
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Cliente Get(int id)
        {
            Cliente res = new Cliente();
            SqlCommand cmd = null;
            SqlDataReader dr = null;
            string query = @"Select Persona.*,
                                    Cliente.Nit, Cliente.Razon, Cliente.FechaRegistro
                            FROM Persona 
                            INNER JOIN Cliente ON Persona.IdPersona=Cliente.IdPersona
                            WHERE Cliente.IdPersona=@id";
            try
            {
                cmd = Methods.CreateBasicCommand(query);
                cmd.Parameters.AddWithValue("@id", id);
                dr = Methods.ExecuteDataReaderCommand(cmd);
                while (dr.Read())
                {
                    res = new Cliente()
                    {
                        IdPersona = dr.GetInt32(0),
                        Ci = dr.GetString(1),
                        Nombre = dr.GetString(2),
                        PrimerApellido = dr.GetString(3),
                        SegundoApellido = dr.GetString(4),
                        Sexo = SexoDal.Get(dr.GetInt32(5)),
                        FechaNacimiento = dr.GetDateTime(6),
                        Usuario = UsuarioDal.Get(dr.GetInt32(7)),
                        Borrado = dr.GetBoolean(8),
                        Direcciones = DireccionDal.GetList(dr.GetInt32(0)),
                        Telefonos = TelefonoDal.GetList(dr.GetInt32(0)),
                        Correos = CorreoDal.GetList(dr.GetInt32(0)),
                        Nit = dr.GetString(9),
                        Razon = dr.GetString(10),
                        FechaRegistro = dr.GetDateTime(11)
                    };
                }
            }
            catch (Exception ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "Obtenet(Get)", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return res;
        }
        
        
        /// <summary>
        /// Obtiene un Cliente de la base de datos
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        public static Cliente GetCI(string ci)
        {
            Cliente res = new Cliente();
            SqlCommand cmd = null;
            SqlDataReader dr = null;
            string query = @"Select Persona.*,
                                    Cliente.Nit, Cliente.Razon, Cliente.FechaRegistro
                            FROM Persona 
                            INNER JOIN Cliente ON Persona.IdPersona=Cliente.IdPersona
                            WHERE Persona.Ci=@ci";
            try
            {
                cmd = Methods.CreateBasicCommand(query);
                cmd.Parameters.AddWithValue("@ci", ci);
                dr = Methods.ExecuteDataReaderCommand(cmd);
                while (dr.Read())
                {
                    res = new Cliente()
                    {
                        IdPersona = dr.GetInt32(0),
                        Ci = dr.GetString(1),
                        Nombre = dr.GetString(2),
                        PrimerApellido = dr.GetString(3),
                        SegundoApellido = dr.GetString(4),
                        Sexo = SexoDal.Get(dr.GetInt32(5)),
                        FechaNacimiento = dr.GetDateTime(6),
                        Usuario = UsuarioDal.Get(dr.GetInt32(7)),
                        Borrado = dr.GetBoolean(8),
                        Direcciones = DireccionDal.GetList(dr.GetInt32(0)),
                        Telefonos = TelefonoDal.GetList(dr.GetInt32(0)),
                        Correos = CorreoDal.GetList(dr.GetInt32(0)),
                        Nit = dr.GetString(9),
                        Razon = dr.GetString(10),
                        FechaRegistro = dr.GetDateTime(11)
                    };
                }
            }
            catch (Exception ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "Obtenet(Get)", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return res;
        }

        /// <summary>
        /// Obtiene Lista de Personal
        /// </summary>
        /// <returns>Lista de Objetos Personal</returns>
        public static List<Cliente> GetList()
        {
            List<Cliente> res = new List<Cliente>();

            SqlCommand cmd = null;
            SqlDataReader dr = null;
            string query = @"SELECT Cliente.* FROM Cliente
                            INNER JOIN Persona ON Cliente.IdPersona=Persona.IdPersona 
                            WHERE Persona.Borrado=0";

            try
            {
                cmd = Methods.CreateBasicCommand(query);
                dr = Methods.ExecuteDataReaderCommand(cmd);

                while (dr.Read())
                {
                    int idPersona = dr.GetInt32(0);
                    Persona persona = PersonaDal.Get(idPersona);

                    res.Add(new Cliente()
                    {
                        IdPersona = idPersona,
                        Ci = persona.Ci,
                        Nombre = persona.Nombre,
                        PrimerApellido = persona.PrimerApellido,
                        SegundoApellido = persona.SegundoApellido,
                        Sexo = persona.Sexo,
                        FechaNacimiento = persona.FechaNacimiento,
                        Correos = persona.Correos,
                        Usuario = persona.Usuario,
                        Direcciones = persona.Direcciones,
                        Telefonos = persona.Telefonos,
                        Borrado = persona.Borrado,
                        Nit = dr.GetString(1),
                        Razon = dr.GetString(2),
                        FechaRegistro = dr.GetDateTime(3)                                
                    });
                }
            }
            catch (SqlException ex)
            {
                Methods.GenerateLogsRelease("ClieneteDal", "ObtenerLista", ex.Message + " " + ex.StackTrace);
                throw ex;
            }
            catch (Exception ex)
            {
                Methods.GenerateLogsRelease("ClienteDal", "ObtenerLista", ex.Message + " " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }

            return res;
        }

    }
}
