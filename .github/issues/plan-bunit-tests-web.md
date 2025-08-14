# Plan: Add bUnit Test Coverage for Web Components and Pages

Created: 2025-08-14 13:30 local

## Background
We want to improve test coverage for the Blazor Server Web project by adding bUnit tests that validate rendering and basic behaviors of Razor components and pages. We will leverage patterns already present in the solution and follow the Beast Mode 3.1 guidance for clear planning, iterative execution, and comprehensive verification.

## Goals
- Establish or expand bUnit tests targeting key components and pages within the `Web` project.
- Ensure tests are reliable, readable, and fast.
- Cover critical UI elements (headers, navigation, lists, forms) and simple logic branches (parameter-driven rendering, visibility, conditional markup).

## Scope
- Components
  - Shared primitives (e.g., PageHeadingComponent)
  - Layout and navigation components
  - Feature components with simple parameterization
- Pages
  - Public pages (Home, About, Contact, Error)
  - Basic authenticated UI surface (as feasible with test providers)

## Non-Goals
- Full end-to-end workflows or deep integration with external services.
- Complex authentication flows beyond what bUnit fakes can reasonably simulate.

## Risks and Considerations
- Components that depend on cascading parameters (e.g., AuthenticationState) may require test providers/stubs.
- Ensure CSS classes/markup are asserted in a resilient way to avoid brittle tests (prefer focused selectors over full markup snapshots).

## Acceptance Criteria
- New bUnit tests exist and run successfully via the solution test runner.
- At least one new test is added in the `Web.Tests.Unit` project as an initial seed to validate setup.
- A representative set of Web components/pages are planned for coverage with a clear, trackable checklist.

## References
- Beast Mode 3.1 guidance (.github/chatmodes/BeastMode.chatmode.md) for planning and iteration.
- bUnit documentation for component testing patterns.

## Todo (Beast Mode-style)
```markdown
- [ ] Inventory Web components/pages and prioritize targets for testing
- [ ] Add initial bUnit test in Web.Tests.Unit for a simple shared component (PageHeadingComponent)
- [ ] Add rendering tests for public pages: Home, About, Contact, Error
- [ ] Add parameterized tests covering different Level/TextColorClass combinations in PageHeadingComponent
- [ ] Add tests for a simple feature list component (render count, item text)
- [ ] Introduce AuthenticationState stubs for pages/components that require it
- [ ] Ensure tests are resilient (selectors over full snapshots, helpful failure messages)
- [ ] Document testing conventions in README or a TESTING.md
```

## Execution Notes
- Iterate in small steps and run tests frequently.
- Prefer minimal dependencies; reuse existing test setup/utilities.
- Keep assertions focused and meaningful.