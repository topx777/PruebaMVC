﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Upds.Sistemas.ProgWeb2.Tintoreria.Core;

namespace Upds.Sistemas.ProgWeb2.Tintoreria.TintoreriaDAL
{
    public class SexoDal
    {
        /// <summary>
        /// Get Sexo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Sexo Get(int id)
        {
            Sexo res = new Sexo();
            SqlCommand cmd = null;
            SqlDataReader dr = null;
            string query = @"SELECT * FROM Sexo WHERE IdSexo=@id";
            try
            {
                cmd = Methods.CreateBasicCommand(query);
                cmd.Parameters.AddWithValue("@id", id);
                dr = Methods.ExecuteDataReaderCommand(cmd);
                while (dr.Read())
                {
                    res = new Sexo()
                    {
                        IdSexo=dr.GetInt32(0),
                        Nombre=dr.GetString(1)
                    };
                }
            }
            catch (Exception ex)
            {
                Methods.GenerateLogsRelease("SexoDal", "Obtenet(Get)", string.Format("{0} {1} Error: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), ex.Message));
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return res;
        }

        /// <summary>
        /// Obtiene Lista de Sexos
        /// </summary>
        /// <returns>Lista de Objetos Sexo</returns>
        public static List<Sexo> GetList()
        {
            List<Sexo> res = new List<Sexo>();

            SqlCommand cmd = null;
            SqlDataReader dr = null;
            string query = @"SELECT * FROM Sexo";

            try
            {
                cmd = Methods.CreateBasicCommand(query);
                dr = Methods.ExecuteDataReaderCommand(cmd);

                while(dr.Read())
                {
                    res.Add(new Sexo()
                    {
                        IdSexo = dr.GetInt32(0),
                        Nombre = dr.GetString(1)
                    });
                }

            }
            catch (SqlException ex)
            {
                Methods.GenerateLogsRelease("SexoDal", "ObtenerLista", ex.Message + " " + ex.StackTrace);
                throw ex;
            }
            catch (Exception ex)
            {
                Methods.GenerateLogsRelease("SexoDal", "ObtenerLista", ex.Message + " " + ex.StackTrace);
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
