using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unicorn.Entities;
using Unicorn.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Unicorn.Controllers
{
    [EnableCors("AllowSales")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Sales")]
    public class SalesController : ControllerBase
    {
        private readonly DataContext _context;
        private Error _errorObject;

        public SalesController(DataContext context)
        {
            this._context = context;
        }

        [HttpGet("allArticles")]
        public async Task<List<ArticleWeb>> AllArticles()
        {
            var articles = await _context.Articles.FromSqlRaw("SELECT * FROM public.\"Articles\"").Include(a => a.Part).ToListAsync();
            var currentDate = DateTime.Now.Date;
            articles = articles.OrderBy(a => a.Id).ToList();
            var articlesWeb = new List<ArticleWeb>();
            foreach(var article in articles)
            {
                var articleWeb = new ArticleWeb()
                {
                    Id = article.Id,
                    SerialNumber = article.Serial,
                    DateManufactured = article.DateManufactured.ToShortDateString()
                };

                if (article.ActionId != null)
                {
                    var action = await _context.Actions.FromSqlRaw("SELECT * FROM public.\"Actions\" WHERE \"Id\" = {0}", article.ActionId).FirstOrDefaultAsync();
                    articleWeb.Action = true;
                    if (action.ActionEnd.CompareTo(currentDate) >= 0 && action.ActionStart.CompareTo(currentDate) <= 0)
                    {
                        var currentAction = 100 - action.DiscountPercent;
                        articleWeb.Price = article.BasePrice * (currentAction / 100);
                    }
                    else if(action.ActionEnd.CompareTo(currentDate) < 0)
                    {
                        var rowCount = await _context.Database.ExecuteSqlRawAsync("UPDATE public.\"Articles\" ŠET \"ActionId\" = {0})", null);
                        if (rowCount == 1)
                        {
                            _context.SaveChanges();
                        }
                        articleWeb.Action = false;
                        articleWeb.Price = article.BasePrice;
                    }
                    else
                    {
                        articleWeb.Price = article.BasePrice;
                    }
                    
                }
                else
                {
                    articleWeb.Action = false;
                    articleWeb.Price = article.BasePrice;
                }

                articlesWeb.Add(articleWeb);

            }

            return articlesWeb;
        }

        [HttpGet("parts")]
        public async Task<List<PartWeb>> PartsAvalaible()
        { 
            var articles = await _context.Articles.FromSqlRaw("SELECT * FROM public.\"Articles\"").Include(a => a.Part).ToListAsync();
            var parts = await _context.Parts.FromSqlRaw("SELECT * FROM public.\"Parts\"").ToListAsync();

            var partsWeb = new List<PartWeb>();

            if (parts.Count != articles.Count)
            {
                foreach (var article in articles)
                {
                    foreach (var part in parts)
                    {
                        if (part.Id == article.PartId)
                        {
                            parts.Remove(part);

                            break;
                        }
                    }
                }

                foreach (var part in parts)
                {
                    var partWeb = new PartWeb()
                    {
                        Id = part.Id,
                        SerialNumber = part.SerialNumber,
                        DateManufactured = part.ManufacterDate.ToShortDateString()
                    };

                    partsWeb.Add(partWeb);
                }
            }

            return partsWeb;
        }

        [HttpPost("newArticle")]
        public async Task<IActionResult> CreateArticle(ArticleWeb newArticle)
        {
            if(ModelState.IsValid)
            {
                var serial = newArticle.SerialNumber;
                var part = await _context.Parts.FromSqlRaw("SELECT * FROM public.\"Parts\" WHERE \"SerialNumber\" = {0}", serial).FirstOrDefaultAsync();

                if(part == null)
                { 
                    return BadRequest(false);
                }

                var article = new Article()
                {
                    PartId = part.Id,
                    BasePrice = newArticle.Price
                };

                var rowCount = await _context.Database.ExecuteSqlRawAsync("INSERT INTO public.\"Articles\" (\"PartId\", \"BasePrice\") VALUES({0}, {1})", part.Id, newArticle.Price);
                if (rowCount == 1)
                {
                    _context.SaveChanges();

                    var articleCreated = await _context.Articles.FromSqlRaw("SELECT * FROM public.\"Articles\" WHERE \"PartId\" = {0}", part.Id).FirstOrDefaultAsync();
                    if(articleCreated == null)
                    {
                        return NotFound(false);
                    }

                    newArticle.DateManufactured = part.ManufacterDate.ToShortDateString();
                    newArticle.Id = articleCreated.Id;
                    return Ok(newArticle);
                }
                else
                {
                    return BadRequest(false);
                }
            }

            return BadRequest(false);
        }

        [HttpPost("newAction")]
        public async Task<IActionResult> CreateAction(ActionWeb newAction)
        {
            if(ModelState.IsValid)
            {
                var startDate = DateTime.Parse(newAction.StartDate);
                var endDate = DateTime.Parse(newAction.EndDate);


                var rowCount = await _context.Database.ExecuteSqlRawAsync("INSERT INTO public.\"Actions\" (\"ActionStart\", \"ActionEnd\", \"DiscountPercent\") VALUES({0}, {1}, {2})", startDate, endDate, newAction.Discount);
                if (rowCount != 1)
                {
                    return BadRequest(false);
                }
                else
                {
                    _context.SaveChanges();
                    var actions = await _context.Actions.FromSqlRaw("SELECT * FROM public.\"Actions\"").ToListAsync();
                    actions = actions.OrderByDescending(a => a.Id).ToList();

                    var actionId = actions.ElementAt(0).Id;
                    newAction.Id = actionId;

                    foreach (var articleId in newAction.Articles)
                    {
                        var rowCount2 = await _context.Database.ExecuteSqlRawAsync("UPDATE public.\"Articles\" SET \"ActionId\" = {0} WHERE \"Id\" = {1}", actionId, articleId);
                        if(rowCount2 != 1)
                        {
                            return BadRequest(false);
                        }
                        _context.SaveChanges();
                    }

                    return Ok(true);
                }
            }

            return BadRequest(false);
        }

        [HttpDelete("deleteArticle/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FromSqlRaw("SELECT * FROM public.\"Articles\" WHERE \"Id\" = {0}", id).Include(a => a.Part).FirstOrDefaultAsync();
            if(article == null)
            {
                return NotFound(false);
            }

            var rowCount = await _context.Database.ExecuteSqlRawAsync("DELETE FROM public.\"Articles\" WHERE \"Id\" = {0}", id);
            if(rowCount == 1)
            {
                _context.SaveChanges();

                return Ok(id);
            }
            else
            {
                return BadRequest(false);
            }
            
        }

        [HttpPut("changeArticlePrice/{id}")]
        public async Task<IActionResult> ChangePrice(int id, NewPriceBody newPrice)
        {
            var article = await _context.Articles.FromSqlRaw("SELECT * FROM public.\"Articles\" WHERE \"Id\" = {0}", id).FirstOrDefaultAsync();
            if (article == null)
            {
                return NotFound(false);
            }

            var rowCount = await _context.Database.ExecuteSqlRawAsync("UPDATE public.\"Articles\" SET \"BasePrice\" = {0} WHERE \"Id\" = {1}", newPrice.NewPrice, id);

            if(rowCount == 1)
            {
                _context.SaveChanges();

                return Ok(true);
            }
            else
            {
                return BadRequest(false);
            }
            
        }

        [HttpPost("createArticles")]
        public void CreateArticles()
        {
            var article1 = new Article()
            {
                PartId = 1,
                BasePrice = 25.5F
            };

            var article2 = new Article()
            {
                PartId = 8,
                BasePrice = 26.5F
            };

            var article3 = new Article()
            {
                PartId = 9,
                BasePrice = 27.5F
            };

            _context.Articles.Add(article1);
            _context.Articles.Add(article2);
            _context.Articles.Add(article3);
            _context.SaveChanges();
        }

    }
}
