using System;
using Domain.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class NavController : Controller
    {
        //
        // GET: /Nav/
        private IProductRepository repository;
        public NavController(IProductRepository repo)
        {
            this.repository = repo;
        }

        public PartialViewResult Menu(string category=null)
        {
            ViewBag.SelectedCategory = category;
            IEnumerable<string> catigories = repository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);
            return PartialView(catigories);
        }

    }
}
