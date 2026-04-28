# Contributing to SmartNotes

Thank you for contributing to SmartNotes. This document outlines the project's standards and how to contribute.

## Guidelines

- Be respectful and constructive in all communications.
- Open an issue before starting larger work to discuss design and scope.

## Branching

- Branch from `main`.
- Use branch names like: `feature/<short-description>`, `fix/<short-description>`, `chore/<short-description>`.

## Commits

- Use present tense: "Add note search" not "Added note search".
- Use conventional commits style where practical (e.g., `feat:`, `fix:`, `chore:`).
- Keep commit messages short and provide details in the PR description.

## Pull Requests

- Target branch: `main`.
- Include a brief summary, motivation, and testing steps.
- Link related issues.
- Ensure all CI checks pass before requesting review.

## Code Style

- Follow the repository .editorconfig (4 spaces, UTF-8, CRLF).
- Prefer `var` when the type is apparent.
- Private fields should be prefixed with an underscore (e.g. `_myField`).
- Keep lines <= 120 characters.

## Testing

- Add unit tests for new logic.
- Aim to keep public APIs covered by tests.
- Run tests locally before pushing: `dotnet test`.

## Continuous Integration

- All PRs must pass automated builds and tests.

## Reporting Issues

- Use the issue template when opening new issues.
- Provide reproduction steps, expected vs actual behavior, and environment details.

Thank you for improving SmartNotes!
