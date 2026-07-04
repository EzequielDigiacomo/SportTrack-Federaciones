# Control de Acceso por Plan SaaS

> **Última actualización:** julio 2026  
> Aplica a: `SportTrack-Federaciones` (backend), `FrontSigdef`, `SportTrack-Front`

---

## Resumen del sistema

El acceso de cada federación a los distintos sistemas y funcionalidades se controla mediante el **plan SaaS** asignado a su entidad en la base de datos. El backend valida el estado de la suscripción (activa, bloqueada, vencida) y los frontends controlan la visibilidad de módulos según los flags del plan.

```
Usuario login
    │
    ▼
Backend (AuthService.LoginAsync)
    ├─ Verifica credenciales
    ├─ Verifica: activo, no bloqueado, no vencido
    ├─ Devuelve AuthResponseDto con:
    │      token, rol, federacionId, clubId
    │      plan: { id, nombre, accesoSigdef, accesoSportTrack,
    │              accesoControlesLive, resultadosTiempoReal, ... }
    │
    ▼
Frontend (AuthContext)
    ├─ Guarda user + plan en localStorage
    ├─ PlanGuard verifica accesoSigdef / accesoSportTrack
    └─ ProtectedRoute verifica accesoControlesLive en rutas avanzadas
```

---

## Niveles de control

### Nivel 1 — Backend: estado de suscripción

Verificado en `AuthService.LoginAsync` y `AuthService.GetMeAsync`:

| Condición             | Error retornado                                      |
|-----------------------|------------------------------------------------------|
| `Activo = false`      | "El acceso de tu institución ha sido suspendido"     |
| `BloqueadoPorFalta`   | "Bloqueado por falta de pago"                        |
| `FechaVencimiento < hoy` | "La suscripción ha vencido"                       |

Esto aplica a **todos los roles excepto SuperAdmin**.

### Nivel 2 — Frontend: sistema permitido por plan

Verificado por `PlanGuard` al entrar a cualquier ruta protegida:

| Flag del plan        | Requerido para acceder a |
|----------------------|--------------------------|
| `accesoSigdef`       | FrontSigdef (SIGDEF)     |
| `accesoSportTrack`   | SportTrack-Front         |

Si el flag es `false` y el usuario no es SuperAdmin → se muestra una **pantalla de plan no compatible** (no un 403 HTTP, es una pantalla UI).

### Nivel 3 — Frontend: features avanzados (Plan L)

Verificado por `ProtectedRoute` en rutas específicas via prop `requiereControlesLive`:

| Flag del plan          | Rutas protegidas en SportTrack-Front       |
|------------------------|--------------------------------------------|
| `accesoControlesLive`  | `/jueces`, `/jueces/largador`             |
|                        | `/jueces/llegada`, `/jueces/carga-manual` |
|                        | `/juez-control/*`                          |

Si el flag es `false` → se muestra pantalla de "Función exclusiva del Plan L".

---

## Matriz de acceso completa

| Plan           | SIGDEF | SportTrack | Tiempo Real | Controles Live |
|----------------|:------:|:----------:|:-----------:|:--------------:|
| SIGDEF (S)     | ✅     | ❌         | ❌          | ❌             |
| SIGDEF (M)     | ✅     | ❌         | ❌          | ❌             |
| SIGDEF (L)     | ✅     | ❌         | ❌          | ❌             |
| SportTrack (S) | ❌     | ✅         | ❌          | ❌             |
| SportTrack (M) | ❌     | ✅         | ✅          | ❌             |
| SportTrack (L) | ❌     | ✅         | ✅          | ✅             |
| Pack Dúo (S)  | ✅     | ✅         | ❌          | ❌             |
| Pack Dúo (M)  | ✅     | ✅         | ✅          | ❌             |
| Pack Dúo (L)  | ✅     | ✅         | ✅          | ✅             |
| SuperAdmin     | ✅     | ✅         | ✅          | ✅             |

> **Nota SIGDEF**: SIGDEF no usa resultados en tiempo real ni controles live (esos son features de SportTrack).

---

## Implementación backend

### `PlanSaaSDto.cs`
`SportTrack-v1.Controladores/SaaS/Dtos/PlanSaaSDto.cs`

Los flags de acceso se calculan **sin migración de BD**, derivados del campo `Nombre`:

```csharp
public bool AccesoSigdef =>
    Nombre.Contains("SIGDEF", StringComparison.OrdinalIgnoreCase) ||
    Nombre.Contains("Dúo", StringComparison.OrdinalIgnoreCase);

public bool AccesoSportTrack =>
    Nombre.Contains("SportTrack", StringComparison.OrdinalIgnoreCase) ||
    Nombre.Contains("Dúo", StringComparison.OrdinalIgnoreCase);

public bool AccesoControlesLive =>
    Nombre.EndsWith("(L)", StringComparison.OrdinalIgnoreCase);
```

### `AuthService.cs`
El `PlanSaaSDto` se incluye en el `AuthResponseDto.Plan` al hacer login:

```csharp
PlanSaaS? planSaaSAsignado = user.Federacion?.PlanSaaS ?? user.Club?.PlanSaaS;
if (planSaaSAsignado != null)
{
    response.Plan = _mapper.Map<PlanSaaSDto>(planSaaSAsignado);
}
```

El token JWT **no contiene el plan** (para no hacerlo demasiado grande). Los datos del plan se retornan en el body del login response.

---

## Implementación frontend

### FrontSigdef

**`src/components/common/PlanGuard.jsx`**
Componente que verifica `user.plan.accesoSigdef`. Si es false, renderiza una pantalla de "Acceso no disponible".

**`src/App.jsx` — `PrivateRoute`**
```jsx
if (user.role !== 'SUPERADMIN') {
    return (
        <PlanGuard requiereSigdef user={user}>
            {children}
        </PlanGuard>
    );
}
```

**`src/context/AuthContext.jsx`**
El campo `plan` se guarda en `loggedUser` durante el login:
```js
plan: response.plan || null
```

---

### SportTrack-Front

**`src/components/Common/PlanGuard.jsx`**
Dos variantes:
- `PlanBloqueado` — pantalla completa (acceso al sistema)
- `PlanBloqueadoFeature` — bloqueo inline (feature de plan L)

**`src/components/Common/ProtectedRoute.jsx`**
```jsx
// Verificar acceso general al sistema SportTrack
if (user?.plan && !user.plan.accesoSportTrack) {
    return <PlanGuard requiereSportTrack user={user}>{children}</PlanGuard>;
}
// Verificar acceso a controles en vivo (solo plan L)
if (requiereControlesLive) {
    return <PlanGuard requiereControlesLive user={user}>{children}</PlanGuard>;
}
```

**`src/App.jsx` — rutas de jueces**
```jsx
<Route path="/jueces/largador" element={
    <ProtectedRoute requiredRole={['Admin','SuperAdmin','Largador']} requiereControlesLive>
        <JudgesLayout><StarterDashboard /></JudgesLayout>
    </ProtectedRoute>
} />
```

**`src/context/AuthContext.jsx`**
El plan se restaura desde `localStorage` al hacer `validateSession()` (ya que `/auth/me` no retorna el plan):
```js
const stored = localStorage.getItem(STORAGE_KEYS.USER_DATA);
if (stored?.plan) normalized.plan = JSON.parse(stored).plan;
```

---

## Agregar un nuevo flag de acceso

1. Agregar propiedad calculada en `PlanSaaSDto.cs`
2. El valor llega automáticamente en el login response (AutoMapper lo serializa)
3. Consumir el flag en el frontend: `user.plan?.nuevoFlag`
4. Crear un nuevo `PlanGuard` o prop en `ProtectedRoute` según corresponda
5. Documentar en `PLANES_SAAS.md`
