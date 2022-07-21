using E_Com.Data;
using E_Com.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace E_Com.Controllers
{
    public class HomeController : Controller
    {
        // variable to add to cart
        public static int cartCount=0;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
           // cartCount = _context.Carts.Where(x=>x.UserId==use).Count(); 
        }

        public IActionResult Popup1()
        {
            return PartialView();
        }

        public IActionResult Index()
        {
            //List<Product> AllCars = _context.Products.ToList();
            //  return View(AllCars);
            cartCount = CartCount();
            var viewModel = new MyViewModel()
            {
                Categories = _context.Categories.ToList(),
                Products = _context.Products.ToList()
            };
            return View(viewModel);
        }
        public IActionResult Buy(int? Id)
        {
            var x = _context.Products.FirstOrDefault(x => x.Id == Id);
            return PartialView("_BuyProductPartial", x);
        }

       // [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([Bind("Id,ProductId,ProductName,Count,Discount,UserId,Status")] Cart cart,int proId,
            string proName, int proPrice )
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null)
            {
                TempData["msg"] = "You Should LogIn first "; // we willl not use beacause we will redirect
                return RedirectToAction(nameof(Index));
            }
            if (!string.IsNullOrEmpty(proName)&& proPrice!=null )
            {
                if(!GetCartProductId(proId, userid))
                {
                    cart.ProductId = proId;
                    cart.Price = proPrice; //count equal price
                    cart.ProductName = proName;
                    cart.UserId = userid;
                    cart.Status = false;
                    _context.Add(cart);
                    await _context.SaveChangesAsync();
                    cartCount = CartCount();// to print how many product in cart
                    //return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["msg"] = proName + " Already Added In Your Cart";
                }

            }
             return RedirectToAction(nameof(Index));
          //   return RedirectToAction("functionName",new (id =PostId);
        }
         public int CartCount()
        {
            int i = 0;
            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(id!=null)
            {
                i = _context.Carts.Where(x => x.UserId == id && x.Status==false).Count();
           
            }else
            {
                i = 0;
            }

            return i;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteCart(int? id, string userid)
        {
            if(id==null)
            {
                return NotFound();
            }
            var cart = _context.Carts.FirstOrDefault(x => x.Id == id);
            if(cart==null)
            {
                return NotFound();
            }
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            cartCount = CartCount();
            return RedirectToAction("Cart", new { id = userid });
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetCartId(int? id, string userid) //to get cart id and used it in payment section
        {
            if (id != null)
            {
                TempData["CartId"]=id;
            }
            return RedirectToAction("Cart",new {id=userid});
        }
    
        //Cart Function
        public async Task<IActionResult> Cart(string id)
        {
            if(id !=null)
            {
                ViewBag.Cart = await GetCart(id);
               /* ViewBag.Pay = await GetPayment(id);*/
                ViewBag.Bill = await GetBill(id);
            }
            
            return View();
        }
        public async Task<List<Cart>> GetCart(string userId)
        {
            
            return await _context.Carts.Where(x=>x.UserId==userId && x.Status==false).ToListAsync();
        }
        public async Task<List<Payment>> GetPayment(string userId)
        {
            return await _context.Payments.Where(x => x.UserId == userId).ToListAsync();
        }
        public async Task<List<BillingAddress>> GetBill(string userId)
        {
            return await _context.BillingAddresses.Where(x => x.UserId == userId).ToListAsync();
        }
        public string GetNameofProduct(int? id)
        {
            string name = "";
             name= _context.Products.FirstOrDefault(x => x.Id == id).ToString();
            return name;
        }
      
        public bool  GetCartProductId(int ProductId, string id)//to check if the product allready in my cart, id user 
        {
            return  _context.Carts.Where(s=>s.UserId==id && s.Status==false).Any(x=>x.ProductId==ProductId); //Any return true or false
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(
               int billId, string firstname, string lastname, string username, string email, string address, string country, int zip,
               string cardtype, string cardname, long cardnumber, DateTime expire, int cvv, int cartId, string userId ,int total)
        {
           if (!UserExists(userId))
            {
                return RedirectToAction(nameof(Index));
            }
            if (!CartExists(cartId))
            {
                TempData["fail"] = "يجب ملئ جميع البيانات بمربعات النص";
                   return RedirectToAction("Cart", new { id = userId });
               // return RedirectToAction(nameof(Cart));
            }

            bool execute = false;
            username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (firstname != null && lastname != null && username != null && email != null && address != null && country != null 
                && zip > 0 && cardtype != null && cardname != null && cardnumber > 0 && expire != null && cvv > 0)
            {
                if (_context.BillingAddresses.Where(x => x.UserId == userId).Count() < 1) //to check if the user have any old checkout to put all his information
                {
                    BillingAddress bill = new BillingAddress();
                    bill.Address = address;
                    bill.Country = country;
                    bill.Email = email;
                    bill.FirstName = firstname;
                    bill.LastName = lastname;
                    bill.UserName = username;
                    bill.Zip = zip;
                    bill.UserId = userId;
                    bill.cvv = cvv;
                    bill.expiration = expire;
                    bill.cardName = cardname;
                    bill.cardNumber = cardnumber;
                    bill.cardType = cardtype;

                    _context.Add(bill);
                    await _context.SaveChangesAsync();
                    billId = bill.Id;
                    execute = true;
                }
                else
                {
                    var bill = await _context.BillingAddresses.FirstOrDefaultAsync(x => x.Id == billId && x.UserId == userId);
                    if (bill != null) //by default bill have data because we are in else , but to have more secure
                    {
                        billId = bill.Id;
                        if (bill.Address != address || //to compare between data in db and data from myform 
                            bill.Country != country ||
                            bill.Email != email ||
                            bill.FirstName != firstname ||
                            bill.LastName != lastname ||
                            bill.UserName != username ||
                            bill.Zip != zip)
                        {
                            bill.Address = address;
                            bill.Country = country;
                            bill.Email = email;
                            bill.FirstName = firstname;
                            bill.LastName = lastname;
                            bill.UserName = username;
                            bill.Zip = zip;
                            bill.UserId = userId;
                            bill.cvv = cvv;
                            bill.expiration = expire;
                            bill.cardName = cardname;
                            bill.cardNumber = cardnumber;
                            bill.cardType = cardtype;

                            _context.BillingAddresses.Attach(bill); //to update data in db
                            _context.Entry(bill).Property(x => x.Address).IsModified = true;
                            _context.Entry(bill).Property(x => x.Country).IsModified = true;
                            _context.Entry(bill).Property(x => x.Email).IsModified = true;
                            _context.Entry(bill).Property(x => x.FirstName).IsModified = true;
                            _context.Entry(bill).Property(x => x.LastName).IsModified = true;
                            _context.Entry(bill).Property(x => x.UserName).IsModified = true;
                            _context.Entry(bill).Property(x => x.Zip).IsModified = true;
                            _context.Entry(bill).Property(x => x.UserId).IsModified = false;
                            _context.Entry(bill).Property(x => x.cvv).IsModified = true;
                            _context.Entry(bill).Property(x => x.expiration).IsModified = true;
                            _context.Entry(bill).Property(x => x.cardName).IsModified = true;
                            _context.Entry(bill).Property(x => x.cardNumber).IsModified = true;
                            _context.Entry(bill).Property(x => x.cardType).IsModified = true;


                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            else
            {
                TempData["fail"] = "You should enter all feilds";
                 return RedirectToAction("Cart", new { id = userId });
                //return RedirectToAction(nameof(Cart));
            }

            if (!BillExists(billId))
            {
                TempData["fail"] = "You should enter all feilds";
                 return RedirectToAction("Cart", new { id = userId });
               // return RedirectToAction(nameof(Cart));
            }

            if (execute || _context.BillingAddresses.Where(x => x.UserId == userId).Count() > 0)
            {
               // if (cardtype != null && cardname != null && cardnumber > 0 && expire != null && cvv > 0)
               // {
                     Payment pay = new Payment();
                pay.billingId = billId;
                pay.Total = total;
                pay.cartId = cartId;

                pay.UserId = userId;

                _context.Add(pay);
                await _context.SaveChangesAsync();

                //change status to ´cart on pay
                var UpdateStatuseOfCart = _context.Carts.FirstOrDefault(x => x.Id == cartId);
                UpdateStatuseOfCart.Status = true;
                //DateTime currentDateTime = DateTime.Now;
                UpdateStatuseOfCart.DateDone = DateTime.Now; 
                _context.SaveChanges();

                cartCount = CartCount();

                TempData["success"] = "save successd";
                   /* Payment pay = new Payment();
                    pay.billingId = billId;
                    pay.Total = total;
                    pay.cartId = cartId;
                
                    pay.UserId =userId;

                    _context.Add(pay);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "save successd";*/
              //  }
               // else
              //  {
                //    TempData["fail"] = "You should enter all feilds";
                  //   return RedirectToAction("Cart", new { id = userId });
                   // return RedirectToAction(nameof(Cart));
                //}
            }

            return RedirectToAction("Cart", new { id = userId });
           // return RedirectToAction(nameof(Cart));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(x => x.Id == id);
        }
        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }

        private bool BillExists(int id)
        {
            return _context.BillingAddresses.Any(e => e.Id == id);
        }

     /*   private bool UserExists(string id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }*/
        //test-------------------------------------------------
     /*   public IActionResult Details(int? Id)
        {
            Cart Cart = new Cart()
            {
               // Product = _context.Products.FirstOrDefault(x => x.Id == Id),
                Count = 1,
                ProductId = (int)Id
            };
            
            return View(Cart);
        }*/

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> History(string id)
        { 
            if(id != null)
            {
                ViewBag.history = await Gethistory(id);
               // ViewBag.history = await Gethistory(id);
            }
            return View();
        }
        public async Task<List<Cart>> Gethistory(string id)
        {
           /* var query = "  select C.Price,C.ProductName, b.FirstName" 
                      + " FROM[E_ComNews].[dbo].[Carts] as C, [E_ComNews].[dbo].[BillingAddresses] as B"
                      + " where c.UserId = '""' and c.Status = 1";*/

            return await _context.Carts.Where(x => x.UserId == id && x.Status == true).ToListAsync();
        }
      

    }
}