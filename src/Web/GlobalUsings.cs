// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GlobalUsings.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================

#region

global using System.Diagnostics.CodeAnalysis;
global using System.Text.Json;

// Auth0 removed: using Microsoft Identity and cookie authentication instead

global using FluentValidation;

global using Mapster;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;

global using ServiceDefaults;

global using Shared.Abstractions;
global using Shared.Entities;
global using Shared.Models;
global using Shared.Validators;

global using Web.Components;
global using Web.Components.Features.Articles.ArticleCreate;
global using Web.Components.Features.Articles.ArticleDetails;
global using Web.Components.Features.Articles.ArticleEdit;
global using Web.Components.Features.Articles.ArticlesList;
global using Web.Components.Features.Categories.CategoriesList;
global using Web.Components.Features.Categories.CategoryCreate;
global using Web.Components.Features.Categories.CategoryDetails;
global using Web.Components.Features.Categories.CategoryEdit;
global using Web.Data;
global using Web.Extensions;

global using static Shared.Services;

#endregion
