# Changelog

## [1.1.0] - 2024

### Added
- Weather Dashboard with Indian cities weather information
- Redis caching for weather data (10-minute cache expiration)
- Navigation menu to switch between Weather Dashboard and User Management
- Support for viewing weather details: temperature, humidity, pressure, wind speed, and visibility

### Changed
- Modified Dashboard component to include navigation between Weather and User Management views
- Updated App.tsx to pass userRole to Dashboard component
- Improved responsive design for mobile devices

### Features
- **Weather Dashboard**: View weather information for 10 major Indian cities
- **Redis Caching**: Weather data cached for 10 minutes to avoid multiple API calls
- **Graceful Fallback**: Application works without Redis, cache is optional
- **Navigation**: Easy switching between Weather and User Management views

## [1.0.0] - 2024

### Initial Release
- User authentication with JWT
- Role-based access control (Admin/User)
- User CRUD operations
- SQLite database
- React TypeScript frontend
- .NET Core Web API backend
