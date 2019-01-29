﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Upds.Sistemas.ProgWeb2.Tintoreria.Core;

namespace Upds.Sistemas.ProgWeb2.Tintoreria.MVC.Models
{
    public class ClienteModel:PersonaModel
    {
        #region Propiedades
        [Required(ErrorMessage ="El campo Nit es Requerido")]
        [StringLength(25,MinimumLength = 10, ErrorMessage ="El Campo Nit debe tener como minimo 10 Caracteres")]
        [Display(Name ="Nit")]
        public string Nit { get; set; }

        [Required(ErrorMessage ="El Campo Razon es Requerido")]
        [Display(Name ="Razon")]
        public string Razon { get; set; }

        [Display(Name ="Fecha Registro")]
        public DateTime FechaRegistro { get; set; }

        #endregion
    }
}