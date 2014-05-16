﻿using AjourBT.Domain.Abstract;
using AjourBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class ErrorController : Controller
    {
        private IRepository repository;

        //public ErrorController(IRepository repo)
        //{
        //    repository = repo;
        //}

        public ActionResult ShowErrorPage(int statusCode, Exception exception)
        {
            Response.StatusCode = statusCode;
            Console.WriteLine(Response.StatusCode);
            ErrorModel model = new ErrorModel { statusCode = statusCode, Exception = exception, RequestedURL = Request.Path };
            Console.WriteLine("statusCode: " + model.statusCode + ' ' + "requestedUrl: " + ' ' + Request.Path);
            return View(model);
        }
    }
}
