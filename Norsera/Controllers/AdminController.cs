using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Norsera.business.Abstract;
using Norsera.entity;
using Norsera.Extensions;
using Norsera.Identity;
using Norsera.Models;


namespace Norsera.Controllers
{

    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;

        private RoleManager<IdentityRole> _roleManager;

        private UserManager<User> _userManager;

        public AdminController(IProductService productService,
                                ICategoryService categoryService,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<User> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> UserMenage(UserDetailsModel model)
        {
            var context = await _userManager.FindByNameAsync(model.UserName);
            // var user = await _userManager.FindByIdAsync(model.UserId);

            return View(new UserDetailsModel()
            {
                UserId = context.Id,
                UserName = context.UserName,
                FirstName = context.FirstName,
                LastName = context.LastName,
                Email = context.Email,
                EmailConfirmed = context.EmailConfirmed,
                // UserId = user.Id,
                // UserName = user.UserName,
                // FirstName = user.FirstName,
                // LastName = user.LastName,
                // Email = user.Email,
                // EmailConfirmed = user.EmailConfirmed,
            });
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var SelectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i => i.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = SelectedRoles
                });
            }
            return Redirect("~/admin/user/list");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model, string[] SelectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        SelectedRoles = SelectedRoles ?? new string[] { };
                        await _userManager.AddToRolesAsync(user, SelectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user, userRoles.Except(SelectedRoles).ToArray<string>());

                        TempData.Put("message", new AlertMessage()
                        {
                            Title = "Role Changed",
                            Message = "Role has been changed.",
                            AlertType = "success"
                        });
                        return Redirect("/admin/user/list");
                    }
                }
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Role Change Not Saved",
                    Message = "Role change was not saved.",
                    AlertType = "danger"
                });
                return Redirect("/admin/user/list");

            }
            TempData.Put("message", new AlertMessage()
            {
                Title = "An Error Occurred",
                Message = "An error occurred.",
                AlertType = "danger"
            });
            return View(model);
        }
        public IActionResult UserList() //2
        {
            return View(_userManager.Users);
        }

        public IActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> UserCreate(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Registration Successful",
                    Message = "Account creation successful.",
                    AlertType = "success"
                });
                return RedirectToAction("UserList");
            }

            return View(model);

        }


        public async Task<IActionResult> RoleEdit(string id)

        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users)
            {

                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonmembers;
                list.Add(user);

                // if( await _userManager.IsInRoleAsync(user,role.Name))
                // {
                //     members.Add(user);
                // }
                // else
                // {
                //     nonmembers.Add(user);
                // }

            }

            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)

        {
            if (ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

                foreach (var userId in model.IdsToDelete ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }

            return Redirect("/admin/role/" + model.RoleId);
        }

        [HttpPost]
        public async Task<IActionResult> UserDelete(UserDetailsModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "User Get Banned LOL",
                        Message = "User Get Banned LOL",
                        AlertType = "success"
                    });
                    return RedirectToAction("UserList");
                }
                else
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "An Error Occurred",
                        Message = "An error occurred",
                        AlertType = "danger"
                    });
                }
            }
            else
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "User Not Found",
                    Message = "User Not Found",
                    AlertType = "danger"
                });
            }
            return RedirectToAction("UserList");

        }

        [HttpPost]
        public async Task<IActionResult> RoleDelete(RoleEditModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Role has been Deleted",
                        Message = "Role has been Deleted",
                        AlertType = "danger"
                    });
                }
                else
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "An Error Occurred",
                        Message = "An error occurred",
                        AlertType = "danger"
                    });
                }
            }
            else
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "User Not Found",
                    Message = "User Not Found",
                    AlertType = "danger"
                });
            }
            return RedirectToAction("RoleList");
        }

        public IActionResult RoleList()

        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()

        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)

        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "An Error Occurred: Role",
                        Message = "An error occurred: Role.",
                        AlertType = "danger"
                    });
                }
            }
            return View(model);
        }


        public IActionResult ProductList()
        {
            return View(new ProductList()
            {
                Products = _productService.Getall()
            });
        }

        [HttpGet]
        public IActionResult ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ProductCreate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new Product()
                {
                    Name = model.Name,
                    Url = model.Url,
                    Price = model.Price,
                    Describiton = model.Describiton,
                    ImageUrl = model.ImageUrl,

                };
                if (_productService.Create(entity))
                {
                    CreateMessage("Product Added", "success");

                    return RedirectToAction("ProductList");
                }
                CreateMessage(_productService.ErrorMessage, "danger");
                return View(model);


            }

            return View(model);

        }

        [HttpGet]
        public IActionResult ProductEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = _productService.GetByIdWithCategories((int)id);

            if (entity == null)
            {
                return NotFound();
            }

            var model = new ProductModel()
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Url = entity.Url,
                Price = (int)entity.Price,
                Describiton = entity.Describiton,
                ImageUrl = entity.ImageUrl,
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,
                SelectedCategories = entity.ProductCategories.Select(i => i.Category).ToList()
            };

            ViewBag.Categories = _categoryService.Getall();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductModel model, int[] CategoryIds, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var entity = _productService.GetById(model.ProductId);
                if (entity == null)
                {
                    return NotFound();
                }

                entity.Name = model.Name;
                entity.Price = model.Price;
                entity.Url = model.Url;
                entity.Describiton = model.Describiton;
                entity.IsApproved = model.IsApproved;
                entity.IsHome = model.IsHome;

                if (file != null)
                {
                    entity.ImageUrl = file.FileName;
                    var extenion = Path.GetExtension(file.FileName);
                    var randomingName = string.Format($"{Guid.NewGuid()}{extenion}");
                    entity.ImageUrl = randomingName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomingName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }



                if (_productService.Update(entity, CategoryIds))
                {
                    CreateMessage("Product has been updated", "success");

                    return RedirectToAction("ProductList");

                }
                CreateMessage(_productService.ErrorMessage, "danger");


            }
            ViewBag.Categories = _categoryService.Getall();
            return View(model);
        }

        public IActionResult DeleteProduct(int ProductId)
        {
            var entity = _productService.GetById(ProductId);

            if (entity != null)
            {
                _productService.Delete(entity);
            }

            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} named product has been deleted.",
                AlertType = "danger"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);

            return RedirectToAction("ProductList");
        }

        //category
        public IActionResult CategoryList()
        {
            return View(new CategoryListViewModel()
            {
                Categories = _categoryService.Getall()
            });
        }

        [HttpGet]
        public IActionResult CategoryCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new Category()
                {
                    Name = model.Name,
                    Url = model.Url,
                    Describtion = model.Describiton
                };
                _categoryService.Create(entity);

                var msg = new AlertMessage()
                {
                    Message = $"{entity.Name}  category named has been added.",
                    AlertType = "success"
                };
                TempData["message"] = JsonConvert.SerializeObject(msg);

                return RedirectToAction("CategoryList");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult CategoryEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = _categoryService.GetByIdWithProducts((int)id);

            if (entity == null)
            {
                return NotFound();
            }

            var model = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Describiton = entity.Describtion,
                Products = entity.ProductCategories.Select(p => p.Product).ToList()

            };
            return View(model);
        }
        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel model)
        {

            if (ModelState.IsValid)
            {
                var entity = _categoryService.GetById(model.CategoryId);
                if (entity == null)
                {
                    return NotFound();
                }

                entity.Name = model.Name;
                entity.Url = model.Url;
                entity.Describtion = model.Describiton;
                _categoryService.Update(entity);

                var msg = new AlertMessage()
                {
                    Message = $"{entity.Name} category named has been updated.",
                    AlertType = "warning"
                };
                TempData["message"] = JsonConvert.SerializeObject(msg);

                return RedirectToAction("CategoryList");
            }

            return View(model);

        }

        public IActionResult DeleteCategory(int CategoryId)
        {
            var entity = _categoryService.GetById(CategoryId);

            if (entity != null)
            {
                _categoryService.Delete(entity);
            }

            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} category named has been deleted.",
                AlertType = "danger"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);

            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteFromCategory(int ProductId, int CategoryId)
        {
            _categoryService.DeleteFromCategory(ProductId, CategoryId);
            return Redirect("/admin/categories/" + CategoryId);
        }

        private void CreateMessage(string message, string alerttype)
        {
            var msg = new AlertMessage()
            {
                Message = message,
                AlertType = alerttype
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
        }

    }
}