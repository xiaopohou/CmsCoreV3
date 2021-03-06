﻿using CmsCoreV3.Data;
using CmsCoreV3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sakura.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsCoreV3.ViewComponents
{
    public class Posts : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public Posts(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string categoryNames, int pageSize = 4)
        {
            int pageNumber = 1;


            if (!String.IsNullOrEmpty(Request.Query["page"]))
            {
                pageNumber = Convert.ToInt32(Request.Query["page"]);
            }

            var items = await GetItems(categoryNames);
            var maxPageNumber = (items.Count() / pageSize) + 1;
            ViewBag.is404 = false;
            if (pageNumber > maxPageNumber)
            {
                pageNumber = maxPageNumber;
                ViewBag.is404 = true;
            }
            if (pageNumber < 1)
            {
                pageNumber = 1;
                ViewBag.is404 = true;
            }
            
                var pagedData = items.ToPagedList(pageSize, pageNumber);
                return View(pagedData);
          
        }
        public async Task<List<Post>> GetItems(string categoryNames)
        {
            if (categoryNames != null)
            {
                return await Task.FromResult(GetPostsByCategoryNames(categoryNames, 1000).Where(w => w.IsPublished == true).ToList());
            }
            else
            {
                return await Task.FromResult(GetPosts().Where(w => w.IsPublished == true).ToList());
            }
        }
        public IEnumerable<Post> GetPosts()
        {
            var posts = GetAll().OrderByDescending(p => p.CreateDate);
            return posts;
        }
        public virtual IEnumerable<Post> GetAll(params string[] navigations)
        {
            var set = _context.Posts.AsQueryable();
            foreach (string nav in navigations)
                set = set.Include(nav);
            return set.Where(p=> (p.PublishDate.HasValue ? p.PublishDate <= DateTime.Now : true)).AsEnumerable();
        }
        public IEnumerable<Post> GetPostsByCategoryNames(string categoryNames, int count)
        {
            string[] categories;
            if (categoryNames == "")
            {
                categories = new string[0];
            }
            else
            {
                categories = categoryNames.Split(',');
            }

            for (var i = 0; i < categories.Length; i++)
            {
                categories[i] = categories[i].Trim().ToLower();
            }
            var posts = GetPostsByCategoryNames(categories, count);
            return posts;
        }


        public IEnumerable<Post> GetPostsByCategoryNames(string[] categories, int count)
        {
            if (categories.Length > 0)
            {
                return (from pc in _context.PostPostCategories join p in _context.Posts on pc.PostId equals p.Id join c in _context.PostCategories on pc.PostCategoryId equals c.Id where (p.PublishDate.HasValue ? p.PublishDate <= DateTime.Now : true) && (categories.Length > 0 ? categories.Contains(c.Name.ToLower()) : true) orderby p.CreateDate descending select p).Take(count).ToList();
            }
            else
            {
                return (from p in _context.Posts where (p.PublishDate.HasValue ? p.PublishDate <= DateTime.Now : true) orderby p.CreateDate descending select p).Take(count).ToList();
            }
        }
    }
}