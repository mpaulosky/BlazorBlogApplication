// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     GlobalUsings.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests.Unit
// =======================================================

global using System;
global using System.Diagnostics.CodeAnalysis;
global using System.Net;
global using System.Reflection;
global using System.Security.Claims;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Linq;

global using Auth0.AspNetCore.Authentication;

global using Bunit;
global using Bunit.TestDoubles;

global using FluentAssertions;

global using FluentValidation;
global using FluentValidation.Results;

global using JetBrains.Annotations;

global using Microsoft.AspNetCore.Antiforgery;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authorization.Infrastructure;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Rendering;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Cors.Infrastructure;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.OutputCaching;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using MongoDB.Bson;
global using MongoDB.Driver;

global using Moq;

global using NSubstitute;

global using Shared.Abstractions;
global using Shared.Entities;
global using Shared.Fakes;
global using Shared.Helpers;
global using Shared.Models;
global using Shared.Validators;

global using Web.Components.Features.Articles.ArticleCreate;
global using Web.Components.Features.Articles.ArticleDetails;
global using Web.Components.Features.Articles.ArticleEdit;
global using Web.Components.Features.Articles.ArticlesList;
global using Web.Components.Features.Categories.CategoriesList;
global using Web.Components.Features.Categories.CategoryDetails;
global using Web.Components.Features.Categories.CategoryEdit;
global using Web.Components.Shared;
global using Web.Data;
global using Web.Data.Auth0;
global using Web.Infrastructure;

global using Xunit;

global using static Shared.Services;
global using static Shared.Helpers.Helpers;
