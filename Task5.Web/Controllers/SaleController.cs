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
using Task5.Web.Models.Client;
using Task5.Web.Models.Filters;
using Task5.Web.Models.Order;
using Task5.Web.Models.Product;
using Task5.Web.Util;

namespace Task5.Web.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private IOrderService orderService;
        private IClientService clientService;
        private IProductService productService;
        private ILogger logger;
        private IMapper mapper;
        public SaleController(IOrderService orderService, IClientService clientService, IProductService productService, ILogger logger)
        {
            this.orderService = orderService;
            this.productService = productService;
            this.clientService = clientService;
            this.logger = logger;
            mapper = new Mapper(AutoMapperWebConfig.Configure());
        }
        [HttpGet]
        public ActionResult Index(int? page)
        {
            ViewBag.CurrentPage = page ?? 1;
            return View();
        }
        [HttpGet]
        public ActionResult Sales(int? page)
        {
            try
            {
                var sales = mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>(orderService.GetOrders());
                ViewBag.CurrentPage = page;
                return PartialView(sales.ToPagedList(page ?? 1, 4));
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sales(OrderFilter model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView();
            }
            try
            {
                var sales = mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>(orderService.GetOrders());
                var expression = model.ToExpression();
                var predicate = expression.Compile();
                if (model.ProductName != null)
                {
                    sales = sales.Where(x => predicate(x.ProductName, model.ProductName));
                }
                if (model.ClientName != null)
                {
                    sales = sales.Where(x => predicate(x.ClientName, model.ClientName));
                }
                if (model.Date != null)
                {
                    sales = sales.Where(x => x.Date == model.Date);
                }
                return PartialView(sales.ToPagedList(1, sales.Count() == 0 ? 1 : sales.Count()));
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Create(int? page)
        {
            try
            {
                var createOrder = new CreateOrderViewModel
                {
                    Clients = new SelectList(mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(clientService.GetClients()), "Id", "Name"),
                    Products = new SelectList(mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(productService.GetProducts()), "Id", "Name")
                };
                ViewBag.CurrentPage = page;
                return View(createOrder);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateOrderViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                model.Clients = new SelectList(mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(clientService.GetClients()), "Id", "Name");
                model.Products = new SelectList(mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(productService.GetProducts()), "Id", "Name");
                return View(model);
            }
            try
            {
                var orderDTO = mapper.Map<CreateOrderViewModel, OrderDTO>(model);
                orderService.Create(orderDTO);
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
                var orderDTO = orderService.GetOrder(x => x.Id == id);
                var editOrder = new EditOrderViewModel
                {
                    ClientId = orderDTO.ClientId,
                    ProductId = orderDTO.ProductId,
                    Clients = new SelectList(mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(clientService.GetClients()), "Id", "Name"),
                    Products = new SelectList(mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(productService.GetProducts()), "Id", "Name")
                };
                ViewBag.CurrentPage = page;
                return View(editOrder);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditOrderViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                model.Clients = new SelectList(mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(clientService.GetClients()), "Id", "Name");
                model.Products = new SelectList(mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(productService.GetProducts()), "Id", "Name");
                return View(model);
            }
            try
            {
                var orderDTO = orderService.GetOrder(x => x.Id == model.Id);
                orderDTO.ClientId = model.ClientId;
                orderDTO.ProductId = model.ProductId;
                orderDTO.Date = model.Date;
                orderService.Update(orderDTO);
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
                var order = mapper.Map<OrderDTO, OrderViewModel>(orderService.GetOrder(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(order);
            }
            catch (Exception ex)
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
                var orderDTO = orderService.GetOrder(x => x.Id == id);
                orderService.Remove(orderDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && orderService != null)
            {
                orderService.Dispose();
                clientService.Dispose();
                productService.Dispose();
                clientService = null;
                productService = null;
                orderService = null;
            }
            base.Dispose(disposing);
        }
    }
}