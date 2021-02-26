using AutoMapper;
using Ninject.Extensions.Logging;
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
        private ILogger logger;

        public ProductController(IProductService productService, IOrderService orderService, ILogger logger)
        {
            this.productService = productService;
            this.orderService = orderService;
            this.logger = logger;
            mapper = new Mapper(AutoMapperWebConfig.Configure());
        }
        [HttpGet]
        public ActionResult Index(int? page)
        {
            ProductFilter filter = (ProductFilter)Session["productFilter"];
            ViewBag.CurrentPage = page ?? 1;
            if (filter != null)
            {
                return View(filter);
            }
            return View(new ProductFilter());
        }
        [HttpGet]
        public ActionResult Products(int? page)
        {
            try
            {
                var products = mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(productService.GetProducts());
                ProductFilter filter = (ProductFilter)Session["productFilter"];
                if (filter != null)
                {
                    products = GetFilteredProducts(products, filter);
                }
                ViewBag.CurrentPage = page;
                return PartialView(products.ToPagedList(page ?? 1, 4));
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult ApplyFilter(ProductFilter model)
        {
            Session["productFilter"] = model;
            return RedirectToAction("Products");
        }
        [HttpGet]
        public ActionResult ClearFilter()
        {
            Session["productFilter"] = null;
            return RedirectToAction("Products");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Create(int? page)
        {
            try
            {
                var createProduct = new CreateProductViewModel();
                ViewBag.CurrentPage = page;
                return View(createProduct);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
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
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id, int? page)
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
            catch(Exception ex)
            {
                logger.Warn(ex.Message);
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
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, int? page, bool? saveChangesError = false)
        {
            try
            {
                var product = mapper.Map<ProductDTO, ProductViewModel>(productService.GetProduct(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(product);
            }
            catch(Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, int? page)
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
        [HttpGet]
        public ActionResult Details(int? id, int? page)
        {
            try
            {
                var product = mapper.Map<ProductDTO, ProductViewModel>(productService.GetProduct(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(product);
            }
            catch(Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult ProductSales(int? id, int? page)
        {
            try
            {
                var orders = mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>
                                       (orderService.GetOrders(x => x.ProductId == id))
                                       .ToPagedList(page ?? 1, 4);
                ViewBag.ProductId = id;
                return PartialView(orders);
            }
            catch(Exception ex)
            {
                logger.Warn(ex.Message);
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
        [HttpGet]
        public JsonResult GetChartData()
        {
            var products = productService.GetProducts();
            var data = products.Select(x => new object[] { x.Name, orderService.GetOrders(y => y.ProductId == x.Id).Count() }).ToArray();
            return Json(data.ToArray(), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<ProductViewModel> GetFilteredProducts(IEnumerable<ProductViewModel> products, ProductFilter filter)
        {
            var expression = filter.ToExpression();
            var predicate = expression.Compile();
            if (filter.Name != null)
            {
                products = products.Where(x => predicate(x.Name, filter.Name));
            }
            if (filter.Price != null)
            {
                products = products.Where(x => x.Price == filter.Price);
            }
            return products;
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