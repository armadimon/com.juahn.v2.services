# Services (com.juahn.v2.services) — DEPRECATED

> ⚠️ **DEPRECATED.** 이 번들 패키지는 도메인별 독립 패키지로 분리되었습니다. 신규 프로젝트는 아래를 사용하세요:
> - **com.juahn.v2.di** — 서비스 로케이터/IoC + 부트스트래퍼
> - **com.juahn.v2.messaging** — 타입 pub/sub 메시지 브로커
> - **com.juahn.v2.timing** — Tick / Time / Coroutine 서비스
>
> (선택) VContainer 연동은 **com.juahn.v2.vcontainer**. 네임스페이스는 동일하게 `Juahn.V2.Services`라 코드 이관 부담이 적습니다.

Layer 1 of the **JuahnFrameworkV2** stack: Unity infrastructure and dependency injection.

This package is **standalone** — it has no dependency on other juahn packages and no dependency on
Newtonsoft.Json or UniTask. Coroutine-based async is used to keep it self-contained.

## What's inside

| Area | Types |
| --- | --- |
| Dependency injection | `IServiceLocator`, `IServiceRegistry`, `ServiceContainer`, `ServiceLocator` (static) |
| Messaging | `IMessage`, `IMessageBroker<T>`, `MessageBroker<T>` |
| Tick | `ITickService`, `TickService`, `TickServiceMonoBehaviour` |
| Coroutine | `ICoroutineService`, `CoroutineService`, `IAsyncCoroutine`, `IAsyncCoroutine<T>` |
| Time | `ITimeService`, `ITimeManipulator`, `TimeService` |
| Composition root | `ServiceBootstrapper`, `IServiceInstaller`, `CoreServicesInstaller` |

Everything lives in the `Juahn.V2.Services` namespace.

## Install

Add via UPM using the git URL:

```
https://github.com/armadimon/com.juahn.v2.services.git
```

In Unity: **Window > Package Manager > + > Add package from git URL...** and paste the URL above.

## Quick usage

Add a `ServiceBootstrapper` to a game object in your first scene (it installs the tick, coroutine and time
services automatically and survives scene loads). Then resolve services anywhere:

```csharp
using Juahn.V2.Services;

// Resolve a core service
var tick = ServiceLocator.Resolve<ITickService>();
tick.SubscribeOnUpdate(dt => Debug.Log($"tick {dt}"), deltaTime: 1f, realTime: true);

// A typed message
public readonly struct PlayerDied : IMessage { }

var broker = new MessageBroker<PlayerDied>();
ServiceLocator.Bind<IMessageBroker<PlayerDied>>(broker);

broker.Subscribe(OnPlayerDied);      // instance method
broker.PublishSafe(new PlayerDied()); // safe against subscribe/unsubscribe during publish
```

Bind your own services with an installer:

```csharp
public sealed class GameInstaller : IServiceInstaller
{
    public void InstallBindings(IServiceRegistry registry)
    {
        registry.Bind<IScoreService>(new ScoreService());
        registry.BindFactory<IReportService>(() => new ReportService()); // lazy, created on first resolve
    }
}

// Register before Awake runs (e.g. on a subclass override, or on a freshly instantiated bootstrapper)
bootstrapper.AddInstaller(new GameInstaller());
```

## Role in the 3-layer stack

```
Layer 3  Gameplay / feature packages
Layer 2  Domain packages (pure logic, no engine)
Layer 1  com.juahn.v2.services  <-- this package (engine + DI + infrastructure)
```

Layer 1 owns everything that touches `UnityEngine` and the DI container. Higher layers register their
services through `IServiceInstaller` at the `ServiceBootstrapper` and resolve infrastructure (tick, coroutine,
time, messaging) from it, keeping domain logic decoupled from the engine.

## License

MIT — see [LICENSE.md](LICENSE.md).
