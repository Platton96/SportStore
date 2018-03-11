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
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductID=1, Name="P1"},
                new Product{ProductID=2, Name="P2"},
                new Product{ProductID=3, Name="P3"},
                new Product{ProductID=4,Name="P4"},
                new Product{ProductID=5, Name="P5"}
            }.AsQueryable);
            ProductController controler = new ProductController(mock.Object);
            controler.PageSize = 3;
            //act
            ProductsListViewModel result = (ProductsListViewModel)controler.List(null,2).Model;
            //assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }
        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //arrange define an Html helper 
            HtmlHelper myHelper = null;
            //arenge pagingIfo
            PagingInfo pagingInfo = new PagingInfo
            {
                CurentPages = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            //arenge delegate
            Func<int, string> pageUrl = i => "Page" + i;

            //act
          MvcHtmlString result= myHelper.PageLinks(pagingInfo, pageUrl);

            //assert

          Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>"
              + @"<a class=""selected"" href=""Page2"">2</a>" 
              + @"<a href=""Page3"">3</a>");

        }
    [TestMethod]
       public void Can_Send_Pagination_View_Model()
       {
        //arrenge
        Mock<IProductRepository> mock = new Mock<IProductRepository>();
        mock.Setup(m => m.Products).Returns(new Product[]{
                new Product {ProductID=1, Name="P1"},
                new Product {ProductID=2, Name="P2"},
                new Product {ProductID=3, Name="P3"},
                new Product {ProductID=4, Name="P4"},
                new Product {ProductID=5, Name="P5"}
        }.AsQueryable());
        //arrenge
        ProductController controller = new ProductController(mock.Object);
        controller.PageSize = 3; 
        //act
        ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;
        
        //asssert
        PagingInfo pageInfo = result.PagingInfo;
        Assert.AreEqual(pageInfo.CurentPages, 2);
        Assert.AreEqual(pageInfo.ItemsPerPage, 3);
        Assert.AreEqual(pageInfo.TotalItems, 5);
        Assert.AreEqual(pageInfo.TotalPages, 2);
       }
      [TestMethod]
       public void Can_Filter_Products()
       {
          // Arrange
          // - create the mock repository
          Mock<IProductRepository> mock = new Mock<IProductRepository>();
          mock.Setup(m => m.Products).Returns(new Product[]{
              new Product{ProductID=1, Name="P1",Category="Cat1"},
              new Product{ProductID=2, Name="P2", Category="Cat2"},
              new Product{ProductID=3, Name="P3", Category="Cat1"},
              new Product{ProductID=4, Name="P4", Category="Cat3"},
              new Product{ProductID=5, Name="P5", Category="Cat1"},
              new Product{ProductID=6, Name="P6", Category="Cat1"}
          }.AsQueryable());
          //arange controller
          ProductController controller = new ProductController(mock.Object);
          controller.PageSize = 3;
          //act
          Product[] result=((ProductsListViewModel)controller.List("Cat1",1).Model).Products.ToArray();
          //assert
          Assert.AreEqual(result.Length, 3);
          Assert.IsTrue(result[0].Name == "P1" && result[0].Category == "Cat1");
          Assert.IsTrue(result[1].Name == "P3" && result[1].Category == "Cat1");
          Assert.IsTrue(result[2].Name == "P5" && result[2].Category == "Cat1");
       }
    [TestMethod]
      public void Can_Create_Categories()
      {
        //arange
        Mock<IProductRepository> mock = new Mock<IProductRepository>();
        mock.Setup(m => m.Products).Returns(new Product[]{
            new Product{ProductID=1, Category="Cat1"},
            new Product{ProductID=2, Category="Cat1"},
            new Product{ProductID=3, Category="Cat2"},
            new Product{ProductID=4, Category="Cat3"}
        }.AsQueryable());
        //arrenge controller
        NavController controller = new NavController(mock.Object);
        //act
        string[] result = ((IEnumerable<string>)controller.Menu().Model).ToArray();
        //assert
        Assert.AreEqual(result.Length, 3);
        Assert.AreEqual(result[0], "Cat1");
        Assert.AreEqual(result[1], "Cat2");
        Assert.AreEqual(result[2], "Cat3");
      }
    [TestMethod]
    public void Indicates_Selected_Category()
    {
        // Arrange
        // - create the mock repository
        Mock<IProductRepository> mock = new Mock<IProductRepository>();
        mock.Setup(m => m.Products).Returns(new Product[] {
                 new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                 new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
                }.AsQueryable());

        // Arrange - create the controller
        NavController target = new NavController(mock.Object);

        // Arrange - define the category to selected
        string categoryToSelect = "Apples";

        // Action
        string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

        // Assert
        Assert.AreEqual(categoryToSelect, result);
    }
    [TestMethod]
    public void Generate_Category_Specific_Product_Count()
    {
        // Arrange
        // - create the mock repository
        Mock<IProductRepository> mock = new Mock<IProductRepository>();
        mock.Setup(m => m.Products).Returns(new Product[] {
    new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
    new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
    new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
    new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
    new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
  }.AsQueryable());

        // Arrange - create a controller and make the page size 3 items
        ProductController target = new ProductController(mock.Object);
        target.PageSize = 3;

        // Action - test the product counts for different categories
        int res1 = ((ProductsListViewModel)target
          .List("Cat1").Model).PagingInfo.TotalItems;
        int res2 = ((ProductsListViewModel)target
          .List("Cat2").Model).PagingInfo.TotalItems;
        int res3 = ((ProductsListViewModel)target
          .List("Cat3").Model).PagingInfo.TotalItems;
        int resAll = ((ProductsListViewModel)target
          .List(null).Model).PagingInfo.TotalItems;

        // Assert
        Assert.AreEqual(res1, 2);
        Assert.AreEqual(res2, 2);
        Assert.AreEqual(res3, 1);
        Assert.AreEqual(resAll, 5);
    }
    }
}
