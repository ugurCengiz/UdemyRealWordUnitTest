using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UdemyRealWordUnitTest.Web.Controllers;
using UdemyRealWordUnitTest.Web.Models;
using UdemyRealWordUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{

    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepository;
        private readonly ProductsController _controller;
        private List<Product> products;

        public ProductControllerTest()
        {
            _mockRepository = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepository.Object);
            products = new List<Product>()
            {
                new Product()
                    { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
                new Product()
                    { Id = 2, Name = "Defter", Price = 200, Stock = 500, Color = "Mavi" }
            };


        }

        //[Fact] 
        public async void Index_ActionExecute_ReturnView()
        {
            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }
        //[Fact]
        public async void Index_ActionExecute_ReturnProductList()
        {
            _mockRepository.Setup(repo => repo.GetAll()).ReturnsAsync(products);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }

        //[Fact]
        public async void Detail_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        //[Fact]
        public async void Detail_IdInvalid_ReturnNotFound()
        {
            Product product = null;

            _mockRepository.Setup(x => x.GetById(0)).ReturnsAsync(product);
            var result = await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        //[Theory]
        //[InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = products.First(x => x.Id == productId);

            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);

            Assert.Equal(product.Name, resultProduct.Name);

        }

        //[Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);

        }

        //[Fact]
        public async void CreatePost_InvalidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir.");

            var result = await _controller.Create(products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);



        }

        //[Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal<string>("Index", redirect.ActionName);


        }

        //[Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;

            _mockRepository.Setup(x => x.Create(It.IsAny<Product>())).Callback<Product>(y => newProduct = y);

            var result = await _controller.Create(products.First());

            _mockRepository.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);

            Assert.Equal(products.First().Id, newProduct.Id);
        }

        //[Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir.");

            var result = await _controller.Create(products.First());

            _mockRepository.Verify(x => x.Create(It.IsAny<Product>()), Times.Never);

        }

        //[Fact]
        public async void Edit_IdIsNull_RedirectToIndexAction()
        {
            var result = await _controller.Edit(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);

        }

        //[Theory]
        //[InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepository.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);

        }

        //[Theory]
        //[InlineData(2)]
        public async void Edit_ActionExecute_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        //[Theory]
        //[InlineData(1)]
        public void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _controller.Edit(2, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        //[Theory]
        //[InlineData(1)]
        public void EditPOST_InValidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "");

            var result = _controller.Edit(productId, products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        //[Theory]
        //[InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _controller.Edit(productId, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        //[Theory]
        //[InlineData(1)]
        public void EditPOST_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepository.Setup(repo => repo.Update(product));

            _controller.Edit(productId, product);

            _mockRepository.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        }

        //[Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        //[Theory]
        //[InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepository.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        //[Theory]
        //[InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepository.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        //[Theory]
        //[InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);

            Assert.IsType<RedirectToActionResult>(result);
        }

        //[Theory]
        //[InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepository.Setup(repo => repo.Delete(product));

            await _controller.DeleteConfirmed(productId);

            _mockRepository.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }


    }
}
