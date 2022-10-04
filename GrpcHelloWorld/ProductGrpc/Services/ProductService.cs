using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductGrpc.Data;
using ProductGrpc.Models;
using ProductGrpc.Protos;
using System;
using System.Threading.Tasks;

namespace ProductGrpc.Services
{
    public class ProductService : ProductProtoService.ProductProtoServiceBase
    {
        private readonly ProductsContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(ProductsContext context, ILogger<ProductService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public override Task<Empty> Test(Empty request, ServerCallContext context)
        {
            return base.Test(request, context);
        }

        public override async Task<ProductModel> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId.Equals(request.ProductId));
            if(product != null)
                return _mapper.Map<ProductModel>(product);

            throw new RpcException(new Status(StatusCode.NotFound, $"Cannot find product with id: {request.ProductId}"));
        }

        public override async Task GetAllProducts(GetAllProductRequest request, IServerStreamWriter<ProductModel> responseStream, ServerCallContext context)
        {
            var products = await _context.Products.ToListAsync();
            foreach(var product in products)
                await responseStream.WriteAsync(_mapper.Map<ProductModel>(product));
        }

        public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            var product = _mapper.Map<Product>(request.Product);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductModel>(product);
        }

        public override async Task<ProductModel> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            try
            {
                var isExist = await _context.Products.AnyAsync(x => x.ProductId.Equals(request.Product.ProductId));
                if (isExist)
                {
                    var product = _mapper.Map<Product>(request.Product);

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<ProductModel>(product);
                }

                return new ProductModel();
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
        {
            var product = await _context.Products.FindAsync(request.ProductId);

            if(product != null)
            {
                _context.Products.Remove(product);
                int isSuccess = await _context.SaveChangesAsync();

                return new DeleteProductResponse { Success = isSuccess > 0};
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"Cannot find product with id: {request.ProductId}"));
        }

        public override async Task<InsertBulkProductResponse> InsertBulkProduct(IAsyncStreamReader<ProductModel> requestStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var product = _mapper.Map<Product>(requestStream.Current);
                _context.Products.Add(product);
            }

            var insertCount = await _context.SaveChangesAsync();

            return new InsertBulkProductResponse { Success = insertCount > 0, InsertCount = insertCount };
        }
    }
}
