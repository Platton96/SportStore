using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Domain.Abstract;
using Domain.Entities;
using WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebUI.HtmlHelpers;
using WebUI.Models;
using System;

namespace UnitTests
{
    [TestClass]
   public class ImageTests
   {
        [TestMethod]
        public void Can_Retrive_Image_Data()
        {
            //arrange
            Product prod =new Product
            {
                ProductID=2,
                Name="Tests",
                ImageData=new byte[]{ },
                ImageMimeType="image/png"

            };
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m=>m.Products).Returns(new Product[]
        {
            new Product{ProductID=1, Name="P1"},
            prod,
            new Product{ProductID=3,Name="P3"}

        }.AsQueryable());
            //arrange -create controller
            ProductController target=new ProductController(mock.Object);
            //act - call the GetImage action method
            ActionResult result=target.GetImage(2);
            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result,typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType,((FileResult)result).ContentType);
        }

   }
}
