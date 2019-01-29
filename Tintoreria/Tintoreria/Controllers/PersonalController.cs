﻿using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Upds.Sistemas.ProgWeb2.Tintoreria.Core;
using Upds.Sistemas.ProgWeb2.Tintoreria.TintoreriaBRL;
using Upds.Sistemas.ProgWeb2.Tintoreria.MVC.Models;

namespace Upds.Sistemas.ProgWeb2.Tintoreria.MVC.Controllers
{
    public class PersonalController : Controller
    {
        // GET: Personal
        [HttpGet]
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            PersonalListModel lista = PersonalListModel.Get();

            IPagedList<PersonalModel> personal = null;
            personal = lista.ToPagedList(pageIndex, pageSize);

            return View(personal);
        }

        // GET: Crear Personal
        public ActionResult Crear()
        {
            CargarSexo();
            CargarTipo();
            CargarCargo();
            PersonalModel personal = new PersonalModel();

            personal.Correos = new List<CorreoModel>();
            personal.Correos.Add(new CorreoModel());

            personal.Telefonos = new List<TelefonoModel>();
            personal.Telefonos.Add(new TelefonoModel());

            personal.Direcciones = new List<DireccionModel>();
            personal.Direcciones.Add(new DireccionModel());

            return View(personal);
        }



        public void CargarSexo()
        {
            ViewBag.ListaSexos = new SelectList(
            (
                from t in SexoController.ListaSexo
                select new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.IdSexo.ToString()
                }
            )
            , "Value", "Text");
        }

        [HttpPost]
        public ActionResult Crear(PersonalModel model)
        {

            Personal per = new Personal()
            {
                IdPersona = model.IdPersona,
                Ci = model.Ci,
                Nombre = model.Nombre,
                PrimerApellido = model.PrimerApellido,
                SegundoApellido = model.SegundoApellido,
                Sexo = new Sexo()
                {
                    IdSexo = model.Sexo.IdSexo,
                    Nombre = model.Sexo.Nombre
                },
                FechaNacimiento = model.FechaNacimiento.Value,
                CodPersonal = model.CodPersonal,
                FechaIngreso = model.FechaIngreso,
                Sueldo = model.Sueldo,
                Correos = null,
                Telefonos = null,
                Direcciones = null,
                Cargo = new Cargo()
                {
                    IdCargo = model.Cargo.IdCargo,
                    Nombre = model.Cargo.Nombre
                },
                Borrado = model.Borrado,
                Usuario = new Usuario()
                {
                    IdUsuario = model.Usuario.IdUsuario,
                    Username = model.Usuario.Username,
                    Password = model.Usuario.Password,
                    EsAdmin = model.Usuario.EsAdmin
                }
            };

            try
            {
                PersonalBrl.Insertar(per);

                return RedirectToAction("../Personal/Index");
            }
            catch
            {
                return View(model);
            }

        }

        public void CargarTipo()
        {
            ViewBag.ListaTipo = new SelectList(
            (
                from t in TipoController.TipoList
                select new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.IdTipo.ToString()
                }
           )
           , "Value", "Text");
        }

        public void CargarCargo()
        {
            ViewBag.ListaCargo = new SelectList(
            (
                from t in CargoController.CargoList
                select new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.IdCargo.ToString()
                }
           )
           , "Value", "Text");
        }

        public ActionResult Editar(int mCodigo)
        {

            Personal per = PersonalBrl.Get(mCodigo);
            PersonalModel model = new PersonalModel()
            {
                IdPersona = per.IdPersona,
                Ci = per.Ci,
                Nombre = per.Nombre,
                PrimerApellido = per.PrimerApellido,
                SegundoApellido= per.SegundoApellido,
                FechaNacimiento=per.FechaNacimiento,
                CodPersonal=per.CodPersonal,
                FechaIngreso=per.FechaIngreso,
                Sueldo=per.Sueldo,
                
            };

            return View(model);
        }
    }
}