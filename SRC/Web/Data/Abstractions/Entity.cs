// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Entity.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : MyBlog
// Project Name :  Web
// =======================================================

using MongoDB.Bson.Serialization.Attributes;

namespace Web.Data.Abstractions;

/// <summary>
///   Base class for all entities in the domain model.
/// </summary>
[Serializable]
public abstract class Entity
{

	/// <summary>
	///   /// Gets the unique identifier for this entity.
	/// </summary>
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	[BsonElement("_id")]
	public ObjectId Id { get; protected init; } = ObjectId.GenerateNewId();

	/// <summary>
	///   Gets the date and time when this entity was created.
	/// </summary>
	[Display(Name = "Created On")]
	[BsonElement("createdOn")]
	[BsonRepresentation(BsonType.DateTime)]
	public DateTime CreatedOn { get; protected init; } = DateTime.UtcNow;

	/// <summary>
	///   Gets or sets the date and time when this entity was la/// </summary>
	[Display(Name = "Modified On")]
	[BsonElement("modifiedOn")]
	[BsonIgnoreIfNull]
	[BsonIgnoreIfDefault]
	[BsonRepresentation(BsonType.DateTime)]
	public DateTime? ModifiedOn { get; set; }

	/// <summary>
	///   Gets or sets the archived status of the entity.
	/// </summary>
	[Display(Name = "Archived")]
	public bool Archived { get; set; }

}
