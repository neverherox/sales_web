using AutoMapper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Task5.BL.DTO;
using Task5.BL.Services.Contracts;
using Task5.Web.Models;
using Task5.Web.Models.Filters;
using Task5.Web.Models.Product;
using Task5.Web.Util;

namespace Task5.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private IProductService productService;
        private IOrderService orderService;
        private IMapper mapper;
        public ProductController(IProductService productService, IOrderService orderService)
        {
            this.productService = productService;
            this.orderService = orderService;
            mapper = new Mapper(AutoMapperWebConfig.Configure());
        }
        public ActionResult Index(int? page)
        {
            ViewBag.CurrentPage = page ?? 1;
            return View();
        }

        public ActionResult Products(int? page)
        {
            try
            {
                var products = mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(productService.GetProducts());
                ViewBag.CurrentPage = page;
                return PartialView(products.ToPagedList(page ?? 1, 4));
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Products(ProductFilter model)
        {
            try
            {
                var products = mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(productService.GetProducts());
                var expression = model.ToExpression();
                var predicate = expression.Compile();
                if (model.Name != null)
                {
                    products = products.Where(x => predicate(x.Name, model.Name));
                }
                if (model.Price != 0)
                {
                    products = products.Where(x => x.Price == model.Price);
                }
                return PartialView(products.ToPagedList(1, products.Count() == 0 ? 1 : products.Count()));
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }
        [Authorize(Roles = "admin")]
        public ActionResult Create(int? page)
        {
            try
            {
                var createProduct = new CreateProductViewModel();
                ViewBag.CurrentPage = page;
                return View(createProduct);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateProductViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var productDTO = mapper.Map<CreateProductViewModel, ProductDTO>(model);
                productService.Create(productDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id, int? page)
        {
            try
            {
                var productDTO = productService.GetProduct(x => x.Id == id);
                var editProduct = new EditProductViewModel
                {
                    Name = productDTO.Name,
                    Price = productDTO.Price
                };
                ViewBag.CurrentPage = page;
                return View(editProduct);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditProductViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var productDTO = productService.GetProduct(x => x.Id == model.Id);
                productDTO.Name = model.Name;
                productDTO.Price = model.Price;
                productService.Update(productDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id, int? page, bool? saveChangesError = false)
        {
            try
            {
                var product = mapper.Map<ProductDTO, ProductViewModel>(productService.GetProduct(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(product);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int? page)
        {
            try
            {
                var productDTO = productService.GetProduct(x => x.Id == id);
                productService.Remove(productDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch
            {
                return View("Error");
            }
        }

        public ActionResult Details(int id, int? page)
        {
            try
            {
                var product = mapper.Map<ProductDTO, ProductViewModel>(productService.GetProduct(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(product);
            }
            catch
            {
                return View("Error");
            }
        }
        public ActionResult ProductSales(int id, int? page)
        {
            try
            {
                var orders = mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>
                                       (orderService.GetOrders(x => x.ProductId == id))
                                       .ToPagedList(page ?? 1, 1);
                ViewBag.ProductId = id;
                return PartialView(orders);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        public JsonResult CheckProductName(string Name)
        {
            var product = productService.GetProduct(x => x.Name == Name);
            var result = (product == null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChartData()
        {
            var products = productService.GetProducts();
            var data = products.Select(x => new object[] { x.Name, orderService.GetOrders(y => y.ProductId == x.Id).Count() }).ToArray();
            return Json(data.ToArray(), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && productService != null)
            {
                productService.Dispose();
                orderService.Dispose();
                orderService = null;
                productService = null;
            }
            base.Dispose(disposing);
        }
    }
}