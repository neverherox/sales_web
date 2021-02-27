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
    [Authorize(Roles = "user")]
    public class SaleController : Controller
    {
        private IOrderService _orderService;
        private IClientService _clientService;
        private IProductService _productService;
        private ILogger _logger;
        private IMapper _mapper;
        public SaleController(IOrderService orderService, IClientService clientService, IProductService productService, ILogger logger)
        {
            _orderService = orderService;
            _productService = productService;
            _clientService = clientService;
            _logger = logger;
            _mapper = new Mapper(AutoMapperWebConfig.Configure());
        }

        [HttpGet]
        public ActionResult Index(int? page)
        {
            OrderFilter filter = (OrderFilter)Session["orderFilter"];
            ViewBag.CurrentPage = page ?? 1;
            if (filter != null)
            {
                return View(filter);
            }
            return View(new OrderFilter());
        }

        [HttpGet]
        public ActionResult Sales(int? page)
        {
            try
            {
                var sales = _mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>(_orderService.GetOrders());
                OrderFilter filter = (OrderFilter)Session["orderFilter"];
                if (filter != null)
                {
                    sales = GetFilteredSales(sales, filter);
                }
                ViewBag.CurrentPage = page;
                return PartialView(sales.ToPagedList(page ?? 1, 4));
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ApplyFilter(OrderFilter model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Sales", null);
            }
            Session["orderFilter"] = model;
            return RedirectToAction("Sales");
        }

        [HttpGet]
        public ActionResult ClearFilter()
        {
            Session["orderFilter"] = null;
            return RedirectToAction("Sales");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Create(int? page)
        {
            try
            {
                var createOrder = new CreateOrderViewModel
                {
                    Clients = new SelectList(_mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(_clientService.GetClients()), "Id", "Name"),
                    Products = new SelectList(_mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(_productService.GetProducts()), "Id", "Name")
                };
                ViewBag.CurrentPage = page;
                return View(createOrder);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateOrderViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                model.Clients = new SelectList(_mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(_clientService.GetClients()), "Id", "Name");
                model.Products = new SelectList(_mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(_productService.GetProducts()), "Id", "Name");
                return View(model);
            }
            try
            {
                var orderDTO = _mapper.Map<CreateOrderViewModel, OrderDTO>(model);
                _orderService.Create(orderDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id, int? page)
        {
            try
            {
                var orderDTO = _orderService.GetOrder(x => x.Id == id);
                var editOrder = new EditOrderViewModel
                {
                    ClientId = orderDTO.ClientId,
                    ProductId = orderDTO.ProductId,
                    Clients = new SelectList(_mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(_clientService.GetClients()), "Id", "Name"),
                    Products = new SelectList(_mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(_productService.GetProducts()), "Id", "Name")
                };
                ViewBag.CurrentPage = page;
                return View(editOrder);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditOrderViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                model.Clients = new SelectList(_mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(_clientService.GetClients()), "Id", "Name");
                model.Products = new SelectList(_mapper.Map<IEnumerable<ProductDTO>, IEnumerable<ProductViewModel>>(_productService.GetProducts()), "Id", "Name");
                return View(model);
            }
            try
            {
                var orderDTO = _orderService.GetOrder(x => x.Id == model.Id);
                orderDTO.ClientId = model.ClientId;
                orderDTO.ProductId = model.ProductId;
                orderDTO.Date = model.Date;
                _orderService.Update(orderDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, int? page, bool? saveChangesError = false)
        {
            try
            {
                var order = _mapper.Map<OrderDTO, OrderViewModel>(_orderService.GetOrder(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, int? page)
        {
            try
            {
                var orderDTO = _orderService.GetOrder(x => x.Id == id);
                _orderService.Remove(orderDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }
        private IEnumerable<OrderViewModel> GetFilteredSales(IEnumerable<OrderViewModel> sales, OrderFilter filter)
        {
            var expression = filter.ToExpression();
            var predicate = expression.Compile();
            if (filter.ProductName != null)
            {
                sales = sales.Where(x => predicate(x.ProductName, filter.ProductName));
            }
            if (filter.ClientName != null)
            {
                sales = sales.Where(x => predicate(x.ClientName, filter.ClientName));
            }
            if (filter.Date != null)
            {
                sales = sales.Where(x => x.Date == filter.Date);
            }
            return sales;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _orderService != null)
            {
                _orderService.Dispose();
                _clientService.Dispose();
                _productService.Dispose();
                _clientService = null;
                _productService = null;
                _orderService = null;
            }
            base.Dispose(disposing);
        }
    }
}