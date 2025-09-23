// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ApplicationUserDto.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================

namespace Shared.Models;

/// <summary>
/// Data transfer object representing an application user.
/// Contains a subset of properties from <c>Shared.Entities.ApplicationUser</c> used by the UI and APIs.
/// </summary>
public sealed record ApplicationUserDto
{
    /// <summary>
    /// Gets the user id.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user name / login.
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Gets the display name set on the application user.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the email address is confirmed.
    /// </summary>
    public bool EmailConfirmed { get; init; }
}
