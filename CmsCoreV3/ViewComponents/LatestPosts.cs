﻿using CmsCoreV3.Data;
using CmsCoreV3.Models;
using CmsCoreV3.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsCoreV3.ViewComponents
{
    public class LatestPosts : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public LatestPosts(ApplicationDbContext context)
        {
            this._context = context;
        }
   
        public async Task<IViewComponentResult> InvokeAsync(string categoryNames = "", int count = 8, bool IsHomePage=false)
        {
            ViewBag.IsHomePage = IsHomePage;
            var items = await GetItems(categoryNames, count);
            return View(items);
        }
        private async Task<List<Post>> GetItems(string categoryNames, int count)
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

            List<Post> posts = GetPostsInCategoryNames(categories, count).Where(w => w.IsPublished == true).ToList();
            return await Task.FromResult(posts);
        }

        //services postbycategoryNames
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
            var posts = GetPostsInCategoryNames(categories, count);
            return posts;
        }
        public IEnumerable<Post> GetPostsByCategoryNames(string categoryNames, int count, long id)
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
            var posts = GetPostsInCategoryNames(categories, count, id);
            return posts;
        }

        //repository getpostbycategory
        public IEnumerable<Post> GetPostsInCategoryNames(string[] categories, int count)
        {
            if (categories.Length > 0)
            {
                return (from pc in _context.PostPostCategories join p in _context.Posts on pc.PostId equals p.Id join c in _context.PostCategories on pc.PostCategoryId equals c.Id where (p.PublishDate.HasValue ? p.PublishDate <= DateTime.Now : true) && (categories.Length > 0 ? categories.Contains(c.Name.ToLower()) : true) orderby p.CreateDate descending select p).Take(count).ToList();
            }
            else
            {
                return (from p in _context.Posts orderby p.CreateDate descending select p).Take(count).ToList();
            }
        }

        public IEnumerable<Post> GetPostsInCategoryNames(string[] categories, int count, long? id)
        {
            if (categories.Length > 0)
            {
                return (from pc in _context.PostPostCategories join p in _context.Posts on pc.PostId equals p.Id join c in _context.PostCategories on pc.PostCategoryId equals c.Id where (p.PublishDate.HasValue ? p.PublishDate <= DateTime.Now : true) && (categories.Length > 0 ? categories.Contains(c.Name.ToLower()) : true) orderby p.CreateDate descending select p).Where(c => c.Id != id).Take(count).ToList();
            }
            else
            {
                return (from p in _context.Posts where (p.PublishDate.HasValue?p.PublishDate<=DateTime.Now:true) orderby p.CreateDate descending select p).Where(c => c.Id != id).Take(count).ToList();
            }

        }
        


    }
}