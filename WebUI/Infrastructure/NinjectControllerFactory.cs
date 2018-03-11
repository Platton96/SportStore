using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Domain.Entities;
using Domain.Abstract;
using Moq;
using System.Configuration;
using SportsStore.Domain.Concrete;


namespace WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        public NinjectControllerFactory()
        {
            // создание контейнера
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
              ? null
              : (IController)ninjectKernel.Get(controllerType);
        }
        private void AddBindings()
        {
            // конфигурирование контейнера
          /*  Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>{
                new Product {Name ="Footbaal",Price=25},
                new Product {Name="Surf bourd", Price=179},
                new Product {Name="Runnind shoes", Price=95}

            }.AsQueryable());/* создание мок объекта чтоб потом связать его с интерфейсом*/
            ninjectKernel.Bind<IProductRepository>().To<EFProductRepository>();
            
        }
    }
}