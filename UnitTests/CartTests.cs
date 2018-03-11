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
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Item()
        {
            //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            //arrange cart
            Cart target = new Cart();
            //act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] result = target.Lines.ToArray();
            //assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[1].Product, p2);
        }
        [TestMethod]
        public void Can_Add_Quantity_For_Exting_Item()
        {
            //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();

            //act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 11);
            CartLine[] result = target.Lines.ToArray();
            //assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Quantity, 12);
            Assert.AreEqual(result[1].Quantity, 1);

        }
        [TestMethod]
        public void Can_Remove_Lines()
        {
            //arrange test product
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };
            //arrange cart
            Cart target = new Cart();
            target.AddItem(p1, 2);
            target.AddItem(p2, 1);
            target.AddItem(p3, 4);
            target.AddItem(p2, 3);

            //act
            target.RemoveLine(p2);

            //assert
            Assert.AreEqual(target.Lines.Where(e => e.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);

        }
        [TestMethod]
        public void Calculate_Total_Value()
        {
            //arrange
            Product p1 = new Product { ProductID = 1, Price = 50M };
            Product p2 = new Product { ProductID = 2, Price = 100M };

            Cart target = new Cart();

            //act
            target.AddItem(p1, 4);
            target.AddItem(p2, 2);
            target.AddItem(p1, 2);
            decimal result = target.ComputeTotalValue();

            //assert
            Assert.AreEqual(result, 500M);

        }
        [TestMethod]
        public void Can_Clear_Contents()
        {
            //arrange
            Product p1 = new Product { ProductID = 1, Price = 50M };
            Product p2 = new Product { ProductID = 2, Price = 100M };
            //arrange cart
            Cart target = new Cart();
            target.AddItem(p1, 4);
            target.AddItem(p2, 2);
            target.AddItem(p1, 2);

            //act
            target.Clear();

            //aseert
            Assert.AreEqual(target.Lines.Count(), 0);

        }
        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //arrange  mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m=>m.Products).Returns(new Product[]{
                new Product {ProductID=1, Name="P1", Category="Cat1"}
            }.AsQueryable());
            //arrange - create cart
            Cart cart = new Cart();
            //arrange - create controller
            CartController target = new CartController(mock.Object);
            //act
            target.AddToCart(cart, 1, null);
            //assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }
        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //arrange  mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product {ProductID=1, Name="P1", Category="Cat1"}
            }.AsQueryable());
            //arrange - create cart
            Cart cart = new Cart();
            //arrange - create controller
            CartController target = new CartController(mock.Object);
            //act
            RedirectToRouteResult result = target.AddToCart(cart, 1, "myUrl");
            //assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }
        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            // Arrange - create a Cart
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController target = new CartController(null);

            // Act - call the Index action method
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            // Assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
    }
}
