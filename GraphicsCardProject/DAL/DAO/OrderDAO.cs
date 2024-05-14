using GraphicsCardProject.DAL.DomainClasses;
using GraphicsCardProject.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GraphicsCardProject.DAL.DAO
{
    public class OrderDAO
    {
        private readonly AppDbContext _db;
        public OrderDAO(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Order>> GetAll(int id)
        {
            return await _db.Orders!.Where(order => order.CustomerId == id).ToListAsync<Order>();
        }

        public async Task<List<OrderDetailsHelper>> GetOrderDetails(int oid, string email)
        {
            Customer? cust = _db.Customers!.FirstOrDefault(cust => cust.Email == email);

            List<OrderDetailsHelper> allDetails = new();

            var results = from o in _db.Orders
                          join oli in _db.OrderLineItems! on o.Id equals oli.OrderId
                          join pi in _db.Products! on oli.ProductId equals pi.Id
                          where (o.CustomerId == cust!.Id && oli.OrderId == oid)
                          select new OrderDetailsHelper
                          {
                              OrderId = o.Id,
                              CustomerId = cust!.Id,
                              ProductItemId = oli.ProductId,
                              QtyS = oli.QtySold,
                              QtyB = oli.QtyOnBackorder,
                              QtyO = oli.QtyOrdered,
                              ProductCost = oli.SellingPrice,
                              ProductName = pi.ProductName,
                              TotalCost = oli.QtyOrdered * oli.SellingPrice,
                              Tax = (oli.SellingPrice * Convert.ToDecimal(0.13)),
                              AfterTax = ((oli.QtyOrdered * oli.SellingPrice) + (oli.SellingPrice * Convert.ToDecimal(0.13))),
                              DateCreated = o.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                          };
            allDetails = await results.ToListAsync();
            return allDetails;
        }

        public async Task<int> AddOrder(int custid, OrderLineHelper[] selections)
        {
            int orderId = -1;

            // we need a transaction as multiple entities involved
            using (var _trans = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    Order order = new();
                    order.CustomerId = custid;
                    order.OrderDate = System.DateTime.Now;
                    order.OrderAmount = 0.0M;

                    // Calculate the totals and then add the order row to the table
                    foreach (OrderLineHelper selection in selections)
                    {
                        order.OrderAmount += Convert.ToDecimal(selection.Item!.MSRP) * selection.Qty;
                    }
                    await _db.Orders!.AddAsync(order);
                    await _db.SaveChangesAsync();

                    // Add each item to the order items table
                    foreach (OrderLineHelper selection in selections)
                    {
                        OrderLineItem oItem = new();
                        oItem.SellingPrice = Convert.ToDecimal(selection.Item!.MSRP);
                        oItem.OrderId = order.Id;
                        oItem.ProductId = selection.Item!.Id;
                        oItem.QtyOrdered = selection.Qty;

                        var productitem = _db.Products!.FirstOrDefault(gc => gc.Id == oItem.ProductId);

                        /* ENOUGH STOCK - QTY ORDERED <= QTY ON HAND */
                        if (oItem.QtyOrdered <= selection.Item!.QtyOnHand)
                        {
                            selection.Item!.QtyOnHand = selection.Item!.QtyOnHand - selection.Qty;
                            productitem!.QtyOnHand = selection.Item!.QtyOnHand;
                            oItem.QtySold = selection.Qty;
                            oItem.QtyOrdered = selection.Qty;
                        }
                        /* NO STOCK - QTY ORDERED > QTY ON HAND */
                        else if (oItem.QtyOrdered > selection.Item!.QtyOnHand)
                        {
                            productitem!.QtyOnHand = 0;
                            selection.Item!.QtyOnBackOrder += (selection.Qty - selection.Item!.QtyOnHand);
                            productitem.QtyOnBackOrder = selection.Item!.QtyOnBackOrder;
                            oItem.QtySold = selection.Item!.QtyOnHand;
                            oItem.QtyOrdered = selection.Qty;
                            oItem.QtyOnBackorder = oItem.QtyOrdered - selection.Item!.QtyOnHand;
                        }

                        // Update and Modify the Database to Reflect the Change in Stock
                        _db.Entry(productitem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _db.Products!.Update(productitem!);
                        await _db.OrderLineItems!.AddAsync(oItem);
                        await _db.SaveChangesAsync();

                    } // END foreach (OrderLineHelper selection in selections)

                    // Commit the Transaction
                    await _trans.CommitAsync();
                    orderId = order.Id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Do NOT commit the transaction and rollback database
                    await _trans.RollbackAsync();
                }
            }

            return orderId;
        }
    }
}
