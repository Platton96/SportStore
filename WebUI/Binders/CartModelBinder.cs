using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using WebUI.Models;

namespace WebUI.Binders
{
    public class CartModelBinder:IModelBinder
    {
        private const string sessionKey = "Cart";
        public object BindModel (ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //get the from the session
            Cart cart = (Cart)controllerContext.HttpContext.Session[sessionKey];
            if (cart == null)
            {
                cart = new Cart();
                controllerContext.HttpContext.Session[sessionKey] = cart;
            }
            return cart;
        }
    }
}