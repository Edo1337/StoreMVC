using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CReshetka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using StoreMVC.Models;

namespace StoreMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IFileService _fileService;
        private readonly StoreAuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IOrderRepository _orderRepos;

        public IFormFile ImageFile { get; set; }
        public AdminController(
            StoreAuthDbContext context,
            UserManager<ApplicationUser> userManager,
            IFileService fileService,
            IOrderRepository orderRepos
            )
        {
            _fileService = fileService;
            _context = context;
            _userManager = userManager;

            _orderRepos = orderRepos;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Products()
        {
            var storeAuthDbContext = _context.Products.Include(p => p.Category);
            return View(await storeAuthDbContext.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View("Edit", new Product());
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,ManufacturerName,Description,Price,Image,CategoryId")] Product product, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                var result = _fileService.SaveImage(ImageFile);
                if (result.Item1 == 1)
                {
                    product.Image = result.Item2;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Products));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,ManufacturerName,Description,Price,Image,CategoryId")] Product product, IFormFile ImageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _fileService.SaveImage(ImageFile);
                    if (result.Item1 == 1)
                    {
                        var oldImage = product.Image;
                        product.Image = result.Item2;
                        var deleteResult = _fileService.DeleteImage(oldImage);
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    ViewBag.Message = string.Format("Изменения в товаре \"{0}\" были сохранены", product.ProductName);
                    return RedirectToAction(nameof(Products));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'StoreAuthDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Products));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderRepos.GetAllOrders();
            return View(orders);
        }
    }
}
