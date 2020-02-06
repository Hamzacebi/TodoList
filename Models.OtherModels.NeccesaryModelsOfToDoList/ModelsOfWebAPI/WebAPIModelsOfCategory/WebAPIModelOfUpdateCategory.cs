﻿using System;
using System.ComponentModel.DataAnnotations;

#region Global Usings
using Commons.CommonOfToDoList.Constants;
#endregion Global Usings

namespace Models.OtherModels.NeccesaryModelsOfToDoList.ModelsOfWebAPI.WebAPIModelsOfCategory
{
    public class WebAPIModelOfUpdateCategory : BaseWebAPIModelOfCategory
    {
        [Required(ErrorMessage = ConstantsOfValidations.CategoryStatusCannotBeEmpty)]
        public bool CategoryStatus { get; set; }
    }
}
