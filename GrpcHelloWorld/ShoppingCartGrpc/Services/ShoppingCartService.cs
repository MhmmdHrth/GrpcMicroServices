using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingCartGrpc.Data;
using ShoppingCartGrpc.Models;
using ShoppingCartGrpc.Protos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartGrpc.Services
{
    public class ShoppingCartService : ShoppingCartProtoService.ShoppingCartProtoServiceBase
    {
        private readonly ShoppingCartContext _context;
        private readonly ILogger<ShoppingCartContext> _logger;
        private readonly IMapper _mapper;

        public ShoppingCartService(ShoppingCartContext context, ILogger<ShoppingCartContext> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<ShoppingCartModel> GetShoppingCart(GetShoppingCartRRequest request, ServerCallContext context)
        {
            var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName == request.Username, context.CancellationToken);

            if (shoppingCart == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={request.Username} can't found"));

            return _mapper.Map<ShoppingCartModel>(shoppingCart);
        }

        public override async Task<AddItemIntoShoppingCartResponse> AddItemIntoShoppingCart(IAsyncStreamReader<AddItemIntoShoppingCartRequest> requestStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName.Equals(requestStream.Current.Username), context.CancellationToken);
                if (shoppingCart == null)
                    throw new RpcException(new Status(StatusCode.NotFound, $"shopping cart with username {requestStream.Current.Username} is not found"));

                var newCartItem = _mapper.Map<ShoppingCartItem>(requestStream.Current.NewCartItem);
                var existingCartItem = shoppingCart.Items.FirstOrDefault(x => x.ProductId.Equals(newCartItem.ProductId));
                if (existingCartItem != null)
                    existingCartItem.Quantity += 1;
                else
                {
                    newCartItem.Price -= 100; //discount
                    shoppingCart.Items.Add(newCartItem);
                }
            }

            var insertCount = await _context.SaveChangesAsync(context.CancellationToken);
            return new AddItemIntoShoppingCartResponse
            {
                InserCount = insertCount,
                Success = insertCount > 0
            };
        }

        public override async Task<ShoppingCartModel> CreateShoppingCart(ShoppingCartModel request, ServerCallContext context)
        {
            var shoppingCart = _mapper.Map<ShoppingCart>(request);
            var isExist = await _context.ShoppingCarts.AnyAsync(x => x.UserName.Equals(request.Username));
            if (isExist)
            {
                _logger.LogError($"Invalid Username For ShoppingCart creation. Username:{request.Username}");
                throw new RpcException(new Status(StatusCode.NotFound, $"shopping cart with username {request.Username} is already exists"));
            }

            await _context.ShoppingCarts.AddAsync(shoppingCart, context.CancellationToken);
            await _context.SaveChangesAsync(context.CancellationToken);

            _logger.LogError($"ShoppingCart successfully created. Username:{request.Username}");

            return _mapper.Map<ShoppingCartModel>(shoppingCart);
        }

        public override async Task<RemoveItemIntoShoppingCartResponse> RemoveItemIntoShoppingCart(RemoveItemIntoShoppingCartRequest request, ServerCallContext context)
        {
            var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName == request.Username, context.CancellationToken);
            if (shoppingCart == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={request.Username} can't found"));

            var removeCartItem = shoppingCart.Items.FirstOrDefault(x => x.ProductId.Equals(request.RemoveCartItem.ProductId));
            if (removeCartItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Cartitem with ProductId={request.RemoveCartItem.ProductId} can't found"));

            shoppingCart.Items.Remove(removeCartItem);
            var count = await _context.SaveChangesAsync(context.CancellationToken);

            return new RemoveItemIntoShoppingCartResponse { Success = count > 0 };
        }
    }
}
