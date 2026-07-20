# Changelog

All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-07-20

### Added

- Initial release of the Layer 1 Services package (`Juahn.V2.Services`).
- Dependency injection: `IServiceLocator`, `IServiceRegistry`, `ServiceContainer`, and static
  `ServiceLocator` with `Bind`, `Bind<T1,T2>`, `Bind<T1,T2,T3>`, `BindFactory`, `Resolve`, `TryResolve`,
  and `Clean`.
- Typed message broker: `IMessage`, `IMessageBroker<T>`, `MessageBroker<T>` with `Publish`, `PublishSafe`
  (copy-iteration plus circular-publish depth guard), `Subscribe`, `Unsubscribe`, and weak-reference
  subscriber cleanup.
- Tick service: `ITickService`, `TickService`, and `TickServiceMonoBehaviour` with update, late update and
  fixed update subscriptions, a `realTime` flag backed by unscaled time, and a DontDestroyOnLoad driver.
- Coroutine service: `ICoroutineService`, `CoroutineService`, `IAsyncCoroutine`, `IAsyncCoroutine<T>`.
- Time service: `ITimeService`, `ITimeManipulator`, `TimeService` abstracting scaled versus unscaled time
  and hosting the simulation clock.
- Composition root: `ServiceBootstrapper`, `IServiceInstaller`, and `CoreServicesInstaller`.
