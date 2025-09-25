// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IdentityUserAccessor.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web
// =======================================================
using Microsoft.AspNetCore.Identity;

namespace Web.Components.Account;

internal sealed class IdentityUserAccessor
(
		UserManager<ApplicationUser> userManager,
		IdentityRedirectManager redirectManager
)
{

	public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
	{
		ApplicationUser? user = await userManager.GetUserAsync(context.User);

		if (user is null)
		{
			redirectManager.RedirectToWithStatus("Account/InvalidUser",
					$"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
		}

		return user;
	}

}
