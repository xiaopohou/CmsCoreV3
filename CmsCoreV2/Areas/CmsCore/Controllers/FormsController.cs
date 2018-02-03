using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CmsCoreV2.Data;
using CmsCoreV2.Models;
using SaasKit.Multitenancy;
using Z.EntityFramework.Plus;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CmsCoreV2.Areas.CmsCore.Controllers
{
    [Authorize(Roles = "ADMIN,FORM")]
    [Area("CmsCore")]
    public class FormsController : ControllerBase
    {


        public FormsController(ApplicationDbContext context, ITenant<AppTenant> tenant) : base(context, tenant)
        {

        }

        public IActionResult ExportToCsv(long id)
        {
            StringWriter sw = new StringWriter();
            HttpContext.Response.Clear();
            string formName = _context.Forms.FirstOrDefault(f => f.Id == id).Slug;
            var fields = _context.FormFields.Where(f => f.FormId == id).OrderBy(o => o.Position).Select(s => s.Name).ToList().ToArray();
            var fieldCount = fields.Count();
            sw.WriteLine("sep=,");
            sw.WriteLine(string.Join(",", fields));
            Response.Headers.Add("content-disposition", "attachment;filename=" + formName + ".csv");
            Response.ContentType = "text/csv";
            var items = _context.FeedbackValues.Include(t => t.Feedback).Where(f => f.Feedback.FormId == id).OrderBy(o => o.Id).ToList();
            int i = 0;
            foreach (var item in items)
            {

                sw.Write(item.Value + ",");
                i++;
                if (i >= fieldCount)
                {
                    sw.WriteLine();
                    i = 0;
                }
            }
            return Content(sw.ToString(), "text/csv");
        }

        // GET: CmsCore/Forms
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SetFiltered<Form>().Where(x => x.AppTenantId == tenant.AppTenantId).Include(f => f.Language);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Preview(long? id)
        {
            var form = await _context.Forms.Include("FormFields").SingleOrDefaultAsync(m => m.Id == id);
            return View(form);
        }

        // GET: CmsCore/Forms/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var form = await _context.Forms
                .Include(f => f.Language)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        // GET: CmsCore/Forms/Create
        public IActionResult Create()
        {
            ViewData["LanguageId"] = new SelectList(_context.Languages.ToList(), "Id", "NativeName");
            return View();
        }

        // POST: CmsCore/Forms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormName,EmailTo,EmailBcc,EmailCc,Description,Template,ClosingDescription,GoogleAnalyticsCode,IsPublished,LanguageId,Id,CreateDate,CreatedBy,UpdateDate,UpdatedBy,Slug,AppTenantId,SendMailToUser,UserMailContent,UserMailSubject,UserMailAttachment,SendSMS1ToUser,UserSMS1,SendSMS2ToUser,UserSMS2")] Form form)
        {
            form.CreatedBy = User.Identity.Name ?? "username";
            form.CreateDate = DateTime.Now;
            form.UpdatedBy = User.Identity.Name ?? "username";
            form.UpdateDate = DateTime.Now;
            form.AppTenantId = tenant.AppTenantId;
            if (ModelState.IsValid)
            {
                _context.Add(form);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["LanguageId"] = new SelectList(_context.Languages.ToList(), "Id", "NativeName", form.LanguageId);
            return View(form);
        }

        // GET: CmsCore/Forms/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var form = await _context.Forms.SingleOrDefaultAsync(m => m.Id == id);
            if (form == null)
            {
                return NotFound();
            }
            ViewData["LanguageId"] = new SelectList(_context.Languages.ToList(), "Id", "NativeName", form.LanguageId);
            return View(form);
        }

        // POST: CmsCore/Forms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("FormName,EmailTo,EmailBcc,EmailCc,Description,Template,ClosingDescription,GoogleAnalyticsCode,IsPublished,LanguageId,Id,CreateDate,CreatedBy,UpdateDate,UpdatedBy,Slug,AppTenantId,SendMailToUser,UserMailContent,UserMailSubject,UserMailAttachment,SendSMS1ToUser,UserSMS1,SendSMS2ToUser,UserSMS2")] Form form)
        {
            if (id != form.Id)
            {
                return NotFound();
            }
            form.UpdatedBy = User.Identity.Name ?? "username";
            form.UpdateDate = DateTime.Now;
            form.AppTenantId = tenant.AppTenantId;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(form);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormExists(form.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["LanguageId"] = new SelectList(_context.Languages.ToList(), "Id", "NativeName", form.LanguageId);
            return View(form);
        }

        // GET: CmsCore/Forms/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var form = await _context.Forms
                .Include(f => f.Language)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        // POST: CmsCore/Forms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var form = await _context.Forms.SingleOrDefaultAsync(m => m.Id == id);
            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool FormExists(long id)
        {
            return _context.Forms.Any(e => e.Id == id);
        }
    }
}
