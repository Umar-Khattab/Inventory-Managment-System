using Inventory_Managment_System.Data;
using Inventory_Managment_System.Interfaces;
using Inventory_Managment_System.Models.Classes;
using Inventory_Managment_System.Repositories;
using Inventory_Managment_System.UnitOfWork;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Managment_System.Models.Services
{
    public class productServices : IProduct
    {
        private readonly IUnitOfWork _unitOfWork;

        public productServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task <Product> createProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            _unitOfWork.Repository<Product>().add(product);
            await _unitOfWork.CompleteAsync();
            return product;

        }
        public void deleteProduct(int id)
        {
            var product = _unitOfWork.Repository<Product>().getByID(id);
            product.isDeleted = true;
            _unitOfWork.Repository<Product>().update(product,id);
            _unitOfWork.Complete();
        }

        public async Task<IEnumerable<Product>> getAllProducts()
        {
            return await _unitOfWork.Repository<Product>().findAllAsync(p => p.isDeleted == false, 15,0);
        }
        public IEnumerable<Product> getProductsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Enumerable.Empty<Product>();

            return _unitOfWork.Repository<Product>().getByName(name);
        }
        public Product getProductById(int id)
        {
            return _unitOfWork.Repository<Product>().getByID(id);
        }
        public void UpdateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var existingProduct = _unitOfWork.Repository<Product>().getByID(product.id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {product.id} not found.");
            }

            // Set the updated date
            product.UpdatedAt = DateTime.Now;
            // Preserve the creation date
            product.createdAt = existingProduct.createdAt;
            // Preserve the isDeleted flag if not explicitly set
            product.isDeleted = existingProduct.isDeleted;

            _unitOfWork.Repository<Product>().update(product, product.id);
            _unitOfWork.Complete();
        }

        public async Task<int> CountProducts()
        {
            return await _unitOfWork.Repository<Product>().countSpecificItems(p=>p.isDeleted==false);  
        }
    }
}
