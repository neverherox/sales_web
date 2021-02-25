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
using Task5.Web.Util;

namespace Task5.Web.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private IClientService clientService;
        private IOrderService orderService;
        private IMapper mapper;
        private ILogger logger;
        public ClientController(IClientService clientService, IOrderService orderService, ILogger logger)
        {
            this.clientService = clientService;
            this.orderService = orderService;
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
        public ActionResult Clients(int? page)
        {
            try
            {
                var clients = mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(clientService.GetClients());
                ViewBag.CurrentPage = page;
                return PartialView(clients.ToPagedList(page ?? 1, 4));
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Clients(ClientFilter model)
        {
            try
            {
                var clients = mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(clientService.GetClients());
                var expression = model.ToExpression();
                var predicate = expression.Compile();
                if (model.Name != null)
                {
                    clients = clients.Where(x => predicate(x.Name, model.Name));
                }
                return PartialView(clients.ToPagedList(1, clients.Count() == 0 ? 1 : clients.Count()));
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
                var createClient = new CreateClientViewModel();
                ViewBag.CurrentPage = page;
                return View(createClient);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateClientViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var clientDTO = mapper.Map<CreateClientViewModel, ClientDTO>(model);
                clientService.Create(clientDTO);
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
                var clientDTO = clientService.GetClient(x => x.Id == id);
                var editClient = new EditClientViewModel
                {
                    Name = clientDTO.Name
                };
                ViewBag.CurrentPage = page;
                return View(editClient);
            }
            catch(Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditClientViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var clientDTO = clientService.GetClient(x => x.Id == model.Id);
                clientDTO.Name = model.Name;
                clientService.Update(clientDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult Details(int? id, int? page)
        {
            try
            {
                var client = mapper.Map<ClientDTO, ClientViewModel>(clientService.GetClient(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(client);
            }
            catch(Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult ClientSales(int? id, int? page)
        {
            try
            {
                var orders = mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>
                                       (orderService.GetOrders(x => x.ClientId == id))
                                       .ToPagedList(page ?? 1, 4);
                ViewBag.ClientId = id;
                return PartialView(orders);
            }
            catch(Exception ex)
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
                var client = mapper.Map<ClientDTO, ClientViewModel>(clientService.GetClient(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(client);
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
                var clientDTO = clientService.GetClient(x => x.Id == id);
                clientService.Remove(clientDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch(Exception ex)
            {
                logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public JsonResult CheckClientName(string Name)
        {
            var client = clientService.GetClient(x => x.Name == Name);
            var result = (client == null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetChartData()
        {
            var clients = clientService.GetClients();
            var data = clients.Select(x => new object[] { x.Name, orderService.GetOrders(y => y.ClientId == x.Id).Count() }).ToArray();
            return Json(data.ToArray(), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && clientService != null)
            {
                clientService.Dispose();
                orderService.Dispose();
                orderService = null;
                clientService = null;
            }
            base.Dispose(disposing);
        }
    }
}