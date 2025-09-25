// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IntegrationTestsCollection.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogApplication
// Project Name :  Web.Tests
// =======================================================

namespace Web.Tests.Integration;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestsCollection : ICollectionFixture<TestPostgresFixture>
{

	// Collection fixture wires TestPostgresFixture to all tests in the collection.

}