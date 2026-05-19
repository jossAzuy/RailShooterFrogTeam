# Changelog
All notable changes to this package will be documented in this file.

## [4.0.1] - 04-03-2026

# Fix
- Reset fixed.

## [4.0.0] - 05-02-2026

# Added
- Support for Volumes.

# Removed
- Removed support for 2022.3.

## [3.0.0] - 17-01-2026

### New
- **Shape-Aware Character Selection**: New selection mode that considers spatial distribution of brightness within each cell, not just overall luminance. Characters now match the actual shape of the image content for dramatically sharper results.
- **Sobel Edge Detection**: Detects edges using Sobel operators and enhances contrast at boundaries for crisper character transitions. Works in both Luminance and ShapeAware modes.
- **Supersampling Anti-Aliasing**: Takes multiple samples per cell (2x2, 3x3, or 4x4) to smooth jagged edges. Eliminates aliasing artifacts at cell boundaries.
- Charsets now automatically generate shape vector data for Shape-Aware mode.

### Changed
- Improved character selection algorithm.
- Better handling of edge cases when charset is not assigned.

## [2.0.4] - 26-12-2025

### New
- 'Use scaled time' in Advanced settings.

## [2.0.3] - 04-09-2025

### Fix
- Fixed shader compilation and build issues.
- Fixed VR build compatibility in Unity 6.

## [2.0.2] - 24-04-2025

# Fix
- Camera 'Post Processing' checkbox fixed.

## [2.0.1] - 11-03-2025

### Fix
- Color precision error.

## [2.0.0] - 04-03-2025

### New
- Support for Unity 6 Render Graph.
- Support for effects in multiples Renderers.

### Removed
- Removed GetSettings(), use .Instance.settings

### Fix
- Small fixes.

## [1.3.0] - 19-10-2024

# Changed
- Support for Unity 2022.3.45f1 LTS and Unity 6000.0.23f1 LTS.
- Updated to Universal RP 14.0.11.
- Removed support for Unity 2021.3 LTS.
- Performance improvements.

## [1.2.0] - 17-07-2024

# Changed
- Removed the AddRenderFeature() and RemoveRenderFeature() from the effect that damaged the configuration file.
- Performance improvements.

# Fix
- Small fixes.

### Changed
- New online documentation.

## [1.1.1] - 02-06-2024

### Changed
- New online documentation.

## [1.1.0] - 10-16-2023

### Fixed
- Unity 2022.1 or newer support.

## [1.0.1] - 30-05-2023

### Fixed
- Chrome and Edge WebGL fix.
- VR fix.

## [1.0.0] - 16-01-2023

- Initial release.