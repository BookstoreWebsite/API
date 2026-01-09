using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Http;
using Microsoft.Extensions.Options;



namespace Bookstore.Infrastructure.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly AppDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EmailSettings _emailSettings;

        public ShoppingCartRepository(AppDbContext context,
                                      IProductRepository productRepository, 
                                      IHttpClientFactory httpClientFactory,
                                      IOptions<EmailSettings> emailOptions) 
        {
            _context = context;           
            _productRepository = productRepository;
            _httpClientFactory = httpClientFactory;
            _emailSettings = emailOptions.Value;
        }

        public async Task AddProductAsync(Guid shoppingCartUserId, Guid productId)
        {       
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ShoppingCartUserId == shoppingCartUserId && ci.ProductId == productId);
            var priceListEntry = await _productRepository.GetCurrentPriceListEntryAsync(productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
            else
            {
                var newItem = new CartItem
                {
                    ShoppingCartUserId = shoppingCartUserId,
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = priceListEntry.Price
                };

                _context.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
        }


        public async Task<List<CartItem>> GetAllCartItemsAsync(Guid shoppingCartUserId)
        {
            var cartItems = await _context.CartItems.Where(ci => ci.ShoppingCartUserId == shoppingCartUserId).ToListAsync();
            foreach (var cartItem in cartItems) 
            {
                cartItem.Product = await GetByProductIdAsync(cartItem.ProductId);
            }
            return cartItems;
        }

        public async Task<Product> GetByProductIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<ShoppingCart> GetByShoppingCartUserIdAsync(Guid id)
        {
            return await _context.ShoppingCarts.FindAsync(id);
        }

        public async Task CreatePurchaseAsync(Guid readerId, Address address)
        {
            var cartItems = await GetAllCartItemsAsync(readerId);

            var purchase = new Purchase
            {
                Id = Guid.NewGuid(),  
                ReaderId = readerId,
                DateTime = DateTime.UtcNow,
                Address = address,
                TotalPrice = 0m
            };

            var purchaseItems = new List<PurchaseItem>(cartItems.Count);
            decimal totalPrice = 0m;

            foreach (var ci in cartItems)
            {
                totalPrice += ci.UnitPrice * ci.Quantity;

                purchaseItems.Add(new PurchaseItem
                {
                    Id = Guid.NewGuid(),
                    PurchaseId = purchase.Id,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                });
            }

            purchase.TotalPrice = totalPrice;

            _context.Purchases.Add(purchase);
            _context.PurchaseItems.AddRange(purchaseItems);

            await _context.SaveChangesAsync();
            await SendPurchaseConfirmationEmail(purchase.Id);
        }

        public async Task ClearShoppingCartAsync(Guid shoppingCartuserId) 
        {
            var cartItems = await GetAllCartItemsAsync(shoppingCartuserId);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem> GetCartItemById(Guid itemId) 
        {
            return await _context.CartItems.FindAsync(itemId);
        }

        public async Task IncrementItemQuantityAsync(Guid itemId) 
        {
            var cartItem = await GetCartItemById(itemId);
            if (cartItem != null) 
            {
                cartItem.Quantity++;
            }
            await _context.SaveChangesAsync();
        }

        public async Task DecrementItemQuantityAsync(Guid itemId)
        {
            var cartItem = await GetCartItemById(itemId);
            if (cartItem != null)
            {
                cartItem.Quantity--;
                if (cartItem.Quantity == 0)
                    _context.CartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(Guid itemId) 
        {
            var cartItem = await GetCartItemById(itemId);
            if(cartItem != null)
                _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();
        }

        private async Task SendPurchaseConfirmationEmail(Guid purchaseId)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Reader)
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            if (purchase == null)
                throw new InvalidOperationException($"Purchase {purchaseId} not found.");

            var productIds = purchase.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var productsById = products.ToDictionary(p => p.Id);

            var lines = new List<OrderLine>();
            foreach (var item in purchase.Items)
            {
                if (!productsById.TryGetValue(item.ProductId, out var product))
                    continue;

                byte[]? imageBytes = await TryDownloadImageAsync(product.ImageUrl);

                lines.Add(new OrderLine
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImageBytes = imageBytes,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            var pdfBytes = BuildPurchasePdf(purchase, purchase.Reader, lines);

            var subject = "Purchase confirmation";
            var bodyText = $"Thank you for your purchase!\n Order ID: {purchase.Id}";

            await SendEmailWithPdfAsync(
                toEmail: purchase.Reader.Email,
                toName: $"{purchase.Reader.FirstName} {purchase.Reader.LastName}",
                subject: subject,
                bodyText: bodyText,
                pdfBytes: pdfBytes,
                pdfFileName: $"Order_{purchase.Id}.pdf"
            );
        }

        private byte[] BuildPurchasePdf(Purchase purchase, User reader, List<OrderLine> lines)
        {
            var address = purchase.Address;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text($"Order Confirmation — {purchase.Id}")
                        .SemiBold().FontSize(16);

                    page.Content().Column(col =>
                    {
                        col.Spacing(12);

                        col.Item().Text($"Thank you for your purchase!").SemiBold().FontSize(13);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Customer").SemiBold();
                                c.Item().Text($"{reader.FirstName} {reader.LastName}");
                                c.Item().Text($"Phone: {reader.PhoneNumber}");
                                c.Item().Text($"Email: {reader.Email}");
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Order").SemiBold();
                                c.Item().Text($"Date: {purchase.DateTime:dd.MM.yyyy HH:mm}");
                                c.Item().Text($"Total: {purchase.TotalPrice:0.00}");
                            });
                        });

                        col.Item().Text("Shipping address").SemiBold();
                        col.Item().Text($"{address.Country}, {address.City}");
                        col.Item().Text($"{address.Street} {address.Number}, {address.PostalCode}");

                        col.Item().LineHorizontal(1);

                        col.Item().Text("Items").SemiBold().FontSize(13);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("");
                                header.Cell().Text("Product").SemiBold();
                                header.Cell().AlignRight().Text("Qty").SemiBold();
                                header.Cell().AlignRight().Text("Unit").SemiBold();
                                header.Cell().AlignRight().Text("Total").SemiBold();

                                header.Cell().ColumnSpan(5).PaddingTop(6).LineHorizontal(1);
                            });

                            foreach (var l in lines)
                            {
                                table.Cell().PaddingVertical(6).AlignMiddle().Element(e =>
                                {
                                    if (l.ProductImageBytes != null)
                                        e.Image(l.ProductImageBytes).FitArea();
                                    else
                                        e.Border(1).BorderColor(Colors.Grey.Lighten2).Height(50).Width(60)
                                         .AlignCenter().AlignMiddle().Text("No image").FontSize(8);
                                });

                                table.Cell().PaddingVertical(6).AlignMiddle().Text(l.ProductName);

                                table.Cell().PaddingVertical(6).AlignMiddle().AlignRight().Text(l.Quantity.ToString());

                                table.Cell().PaddingVertical(6).AlignMiddle().AlignRight().Text($"{l.UnitPrice:0.00}");

                                var lineTotal = l.UnitPrice * l.Quantity;
                                table.Cell().PaddingVertical(6).AlignMiddle().AlignRight().Text($"{lineTotal:0.00}");

                                table.Cell().ColumnSpan(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                            }
                        });

                        col.Item().AlignRight().Text($"Total: {purchase.TotalPrice:0.00}")
                            .SemiBold().FontSize(13);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Bookstore • ")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);

                            text.Span($"Order ID: {purchase.Id}")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                        });
                });
            })
            .GeneratePdf();
        }

        private async Task<byte[]?> TryDownloadImageAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            try
            {
                var client = _httpClientFactory.CreateClient();
                using var resp = await client.GetAsync(imageUrl);
                if (!resp.IsSuccessStatusCode) return null;

                return await resp.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                return null;
            }
        }

        private async Task SendEmailWithPdfAsync(
            string toEmail,
            string toName,
            string subject,
            string bodyText,
            byte[] pdfBytes,
            string pdfFileName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = bodyText
            };

            builder.Attachments.Add(pdfFileName, pdfBytes, new ContentType("application", "pdf"));
            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
