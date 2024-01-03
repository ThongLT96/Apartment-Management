﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Framework.Web.Areas.App.Models.Layout;
using Framework.Web.Session;
using Framework.Web.Views;

namespace Framework.Web.Areas.App.Views.Shared.Themes.Theme11.Components.AppTheme11Brand
{
    public class AppTheme11BrandViewComponent : FrameworkViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public AppTheme11BrandViewComponent(IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var headerModel = new HeaderViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync()
            };

            return View(headerModel);
        }
    }
}
