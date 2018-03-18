﻿using System.Globalization;
using System.Linq;
using AutoMapper;
using blogcore.BLL.Interfaces;
using blogcore.Entities;
using blogcore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace blogcore.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Articles")]
    public class ArticlesController : Controller
    {
        private readonly IArticleBLL _articleBll;
        private readonly IMapper _mapper;

        public ArticlesController(IArticleBLL articleBll, IMapper mapper)
        {
            _articleBll = articleBll;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var userIdParam = HttpContext.Request.Query["userId"].ToList().FirstOrDefault();

            if (userIdParam == null)
            {
                var articles = _articleBll.GetAllArticles();
                var articleVms = articles.Select(article => _mapper.Map<ArticleViewModel>(article));
                return Ok(articleVms);
            }

            if (int.TryParse(userIdParam, NumberStyles.Any, CultureInfo.InvariantCulture, out int userId))
            {
                var articles = _articleBll.GetArticlesByUserId(userId);
                var articleVms = articles.Select(article => _mapper.Map<ArticleViewModel>(article));
                return Ok(articleVms);
            }

            return BadRequest();
        }

        [HttpGet("{id}", Name = "GetArticleById")]
        public IActionResult Get(int id)
        {
            var article = _articleBll.GetArticleById(id);

            if (article == null)
            {
                return NotFound();
            }

            var articleVm = _mapper.Map<ArticleViewModel>(article);

            return Ok(articleVm);
        }

        [HttpPost]
        public IActionResult Post([FromBody] JObject value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            try
            {
                var articleVm = value.ToObject<ArticleViewModel>();
                var article = _mapper.Map<ArticleEntity>(articleVm);

                var id = _articleBll.AddArticle(article);

                if (id == -1)
                {
                    return BadRequest();
                }

                return Created("", new { id });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] JObject value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            try
            {
                var articleVm = value.ToObject<ArticleViewModel>();
                var article = _mapper.Map<ArticleEntity>(articleVm);

                var result = _articleBll.ChangeArticle(id, article);

                if (id == -1)
                {
                    return BadRequest();
                }

                return Ok(new { result });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _articleBll.DeleteArticle(id);
            return Ok(new { result });
        }
    }
}
