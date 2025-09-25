// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Entity.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Shared
// =======================================================
namespace Shared.Abstractions;

/// <summary>
///   Base class for all entities in the domain model.
/// </summary>
[Serializable]
public abstract class Entity
{

	/// <summary>
	///   /// Gets the unique identifier for this entity.
	/// </summary>
	public Guid Id { get; protected init; } = Guid.CreateVersion7();

	/// <summary>
	///   Gets the date and time when this entity was created.
	/// </summary>
	[Display(Name = "Created On")]
	public DateTime CreatedOn { get; protected init; } = DateTime.UtcNow;

	/// <summary>
	///   Gets or sets the date and time when this entity was la///
	/// </summary>
	[Display(Name = "Modified On")]
	public DateTime? ModifiedOn { get; set; }

	/// <summary>
	///   Gets or sets the archived status of the entity.
	/// </summary>
	[Display(Name = "Is Archived")]
	public bool IsArchived { get; set; }

}
