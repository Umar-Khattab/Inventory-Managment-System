using Inventory_Managment_System.Data;
using Inventory_Managment_System.Interfaces;
using Inventory_Managment_System.Models.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_Managment_System.Models.Services
{
    public class productServices : IProduct
    {
        private readonly InventoryDbContext _context;

        public productServices(InventoryDbContext context)
        {
            _context = context;
        }
        public void createProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            _context.products.Add(product);
            _context.SaveChanges();
        }

        public void deleteProduct(int id)
        {
            var product = _context.products.Find(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            _context.products.Remove(product);
            _context.SaveChanges();
        }

        public List<Product> getAllProducts()
        {
            return _context.products.ToList();
        }
        public List<Product> getProductsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Product>();

            return _context.products
                .Include(p => p.supplier)
                .Include(p => p.category)
                .Include(p => p.brand)
                .Where(p => p.name.Contains(name))
                .ToList();
        }
        public Product getProductById(int id)
        {
            return _context.products
                .Include(p => p.supplier)
                .Include(p => p.category)
                .Include(p => p.brand)
                .FirstOrDefault(p => p.id == id);
        }
        public void UpdateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var existingProduct = _context.products.Find(product.id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {product.id} not found.");
            }

            // Update properties
            existingProduct.name = product.name;
            existingProduct.description = product.description;
            existingProduct.price = product.price;
            existingProduct.stockQuantity = product.stockQuantity;
            existingProduct.minimumStockLevel = product.minimumStockLevel;
            existingProduct.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }
    }
}
