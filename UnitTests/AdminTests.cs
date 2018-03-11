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
   public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID=1, Name="P1"},
                    new Product {ProductID=2, Name="P2"},
                    new Product {ProductID=3, Name="P3"}
                }.AsQueryable());
            AdminController target = new AdminController(mock.Object);

            //act
            Product[] result = ((IEnumerable<Product>) target.Index().ViewData.Model).ToArray();
            //assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }
        [TestMethod]
        public void Can_Edit_Product()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID=1, Name="P1"},
                    new Product {ProductID=2, Name="P2"},
                    new Product {ProductID=3, Name="P3"}
                }.AsQueryable());
            AdminController target = new AdminController(mock.Object);
            //act
            Product p1 = (Product)target.Edit(1).ViewData.Model;
            Product p2 = (Product)target.Edit(2).ViewData.Model;
            Product p3 = (Product)target.Edit(3).ViewData.Model;
            //assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }
        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID=1, Name="P1"},
                    new Product {ProductID=2, Name="P2"},
                    new Product {ProductID=3, Name="P3"}
                }.AsQueryable());
            AdminController target = new AdminController(mock.Object);
            //act
            Product result = (Product)target.Edit(4).ViewData.Model;
            //Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a product
            Product product = new Product { Name = "Test" };
            // Act - try to save the product
            ActionResult result = target.Edit(product,null);
            // Assert - check that the repository was called
            mock.Verify(m => m.SaveProducts(product));
            // Assert - check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a product
            Product product = new Product { Name = "Test" };
            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");
            // Act - try to save the product
            ActionResult result = target.Edit(product,null);
            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveProducts(It.IsAny<Product>()), Times.Never());
            // Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
