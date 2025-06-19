# Changelog

All notable changes to the Event System package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-06-19

### Added
- Complete EventBus system with runtime and editor integration
- Persistent event management using ScriptableObjects
- EventBusEventCreator window for creating and managing events (with type selection)
- EventBusEventViewer window for monitoring events in real-time (shows event type)
- EventBusInspector component for scene-specific monitoring and test results
- Support for both type-safe (generic) and non-generic events
- Event domain organization (Core, Gameplay, Input, etc.)
- Auto-loading of persistent events at runtime
- Comprehensive debug logging and error handling
- Example scripts and documentation

### Changed
- Enforced type-safety: each event name can only be used as generic (with payload) or non-generic (no payload), never both
- Editor tools and persistent assets now store and display event type information
- Improved error messages for type conflicts and misuse
- Test script now checks both generic and non-generic event flows and displays results in the inspector
- Updated README and documentation for new workflow and type-safety rules

### Features
- **For Designers**: Visual tools to create, manage, and monitor events, with clear type selection
- **For Developers**: Simple API for subscribing, triggering, and unsubscribing from both generic and non-generic events
- **Persistent Events**: Events created in editor survive scene reloads and Unity restarts
- **Runtime Events**: Dynamic event creation during gameplay
- **Editor Integration**: Seamless Unity editor integration with custom windows and inspectors

### Technical Details
- Namespace: `EventSystem` (runtime), `EventSystem.Editor` (editor)
- Unity Version: 2021.3+
- No external dependencies
- MIT License 