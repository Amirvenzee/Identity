﻿using identityPractice.ViewModels;
using IdentityPractice.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityPractice.Controllers
{
    public class ManageUserController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
       
        private readonly RoleManager<IdentityRole> _roleManager;



        public ManageUserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
         
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = _userManager.Users
                .Select(u => new IndexViewModel()
                { Id = u.Id, UserName = u.UserName, Email = u.Email }).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, string userName)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userName)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.UserName = userName;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return RedirectToAction("index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> AddUserToRole(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = _roleManager.Roles.AsTracking()
                .Select(r => r.Name).ToList();
            var userRoles = await _userManager.GetRolesAsync(user);
            var validRoles = roles.Where(r => !userRoles.Contains(r))
                .Select(r => new UserRolesViewModel(r)).ToList();
            var model = new AddUserToRoleViewModel(id, validRoles);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(AddUserToRoleViewModel model)
        {
            if (model == null) return NotFound();
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var requestRoles = model.UserRoles.Where(r => r.IsSelected)
            .Select(u => u.RoleName)
                .ToList();
            var result = await _userManager.AddToRolesAsync(user, requestRoles);

            if (result.Succeeded) return RedirectToAction("index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveUserFromRole(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var validRoles = userRoles.Select(r => new UserRolesViewModel(r)).ToList();
            var model = new AddUserToRoleViewModel(id, validRoles);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserFromRole(AddUserToRoleViewModel model)
        {
            if (model == null) return NotFound();
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var requestRoles = model.UserRoles.Where(r => r.IsSelected)
            .Select(u => u.RoleName)
                .ToList();
            var result = await _userManager.RemoveFromRolesAsync(user, requestRoles);

            if (result.Succeeded) return RedirectToAction("index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> UpdateSecurityStamp(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.UpdateSecurityStampAsync(user);
            
            return RedirectToAction("Index");
        }
    }
}
