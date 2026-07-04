# Planes SaaS — Definición y Features

> **Última actualización:** julio 2026  
> Aplica a: `SportTrack-Federaciones` (backend), `FrontSigdef`, `SportTrack-Front`

---

## Estructura de planes

El sistema ofrece **9 planes** agrupados en **3 familias** × **3 tallas (S / M / L)**.

| ID | Plan            | Precio  | Sistema       | Talla |
|----|-----------------|---------|---------------|-------|
| 1  | SIGDEF (S)      | $50     | Solo SIGDEF   | S     |
| 2  | SIGDEF (M)      | $120    | Solo SIGDEF   | M     |
| 3  | SIGDEF (L)      | $250    | Solo SIGDEF   | L     |
| 4  | SportTrack (S)  | $40     | Solo SportTrack | S   |
| 5  | SportTrack (M)  | $90     | Solo SportTrack | M   |
| 6  | SportTrack (L)  | $190    | Solo SportTrack | L   |
| 7  | Pack Dúo (S)   | $75     | SIGDEF + SportTrack | S |
| 8  | Pack Dúo (M)   | $170    | SIGDEF + SportTrack | M |
| 9  | Pack Dúo (L)   | $350    | SIGDEF + SportTrack | L |

---

## Tallas: criterio de selección

| Talla | Perfil de federación                   | Límite de atletas | Usuarios aprox. |
|-------|----------------------------------------|-------------------|-----------------|
| **S** | Federación pequeña o regional          | hasta 500         | < 50            |
| **M** | Federación nacional consolidada        | hasta 2.000       | < 300           |
| **L** | Federación grande / multi-disciplina   | ilimitados        | ilimitados      |

---

## Features por sistema y talla

### SIGDEF — Gestión de Federación

| Feature                       | S   | M   | L   |
|-------------------------------|-----|-----|-----|
| Gestión de atletas            | ✅  | ✅  | ✅  |
| Gestión de clubes             | ✅  | ✅  | ✅  |
| Delegados y entrenadores      | ✅  | ✅  | ✅  |
| Exportación a Excel           | ✅  | ✅  | ✅  |
| Límite de atletas             | 500 | 2.000 | ∞ |
| Competencias / eventos        | ∞   | ∞   | ∞   |
| Soporte prioritario           | ❌  | ❌  | ✅  |
| Historial de auditoría        | 30 días | 90 días | ∞ |

### SportTrack — Gestión de Competencias

| Feature                                   | S   | M   | L   |
|-------------------------------------------|-----|-----|-----|
| Gestión de eventos y regatas              | ✅  | ✅  | ✅  |
| Inscripciones y resultados básicos        | ✅  | ✅  | ✅  |
| Exportación a Excel                       | ✅  | ✅  | ✅  |
| Competencias activas simultáneas          | ∞   | ∞   | ∞   |
| **Resultados en tiempo real** (SignalR)   | ❌  | ✅  | ✅  |
| **Panel de Largador** (StarterDashboard)  | ❌  | ❌  | ✅  |
| **Panel de Cronometrista** (FinisherDashboard) | ❌ | ❌ | ✅ |
| **Juez de Control** (JuezControlDashboard) | ❌ | ❌ | ✅ |
| Soporte prioritario                       | ❌  | ❌  | ✅  |

> **Pack Dúo**: mismas features que la talla equivalente, pero con acceso a **ambos sistemas**.

---

## Entidad en base de datos

Tabla: `catalogos."PlanesSaaS"`  
Proyecto: `SportTrack-v1.Entidades/Entidades/PlanSaaS.cs`

```csharp
public class PlanSaaS
{
    public int Id { get; set; }
    public string Nombre { get; set; }       // e.g. "SIGDEF (M)"
    public decimal Precio { get; set; }
    public int MaxAtletas { get; set; }      // -1 = ilimitado
    public int MaxTorneosActivos { get; set; } // -1 = ilimitado (todos)
    public bool ResultadosTiempoReal { get; set; }
    public bool ExportacionExcel { get; set; }
    public bool SoportePrioritario { get; set; }
}
```

---

## Flags de acceso (derivados, sin migración)

Los flags de acceso **no se almacenan en la base de datos**. Se calculan en tiempo real en `PlanSaaSDto.cs` a partir del campo `Nombre`:

```csharp
// AccesoSigdef: planes "SIGDEF (...)" o "Pack Dúo (...)"
public bool AccesoSigdef => Nombre.Contains("SIGDEF") || Nombre.Contains("Dúo");

// AccesoSportTrack: planes "SportTrack (...)" o "Pack Dúo (...)"
public bool AccesoSportTrack => Nombre.Contains("SportTrack") || Nombre.Contains("Dúo");

// AccesoControlesLive: solo planes de talla L (cualquier familia)
public bool AccesoControlesLive => Nombre.EndsWith("(L)");
```

Estos flags se serializan en el **login response** (`AuthResponseDto.Plan`) y los frontends los consumen para controlar el acceso.

---

## Modificar precios o features

Para cambiar un feature de un plan, ejecutar SQL directamente en la BD:

```sql
-- Ejemplo: activar ResultadosTiempoReal para todos los planes S
UPDATE catalogos."PlanesSaaS"
SET "ResultadosTiempoReal" = true
WHERE "Nombre" LIKE '%(S)';
```

Para agregar un nuevo campo de feature al plan, seguir:
1. Agregar propiedad a `PlanSaaS.cs`
2. Agregar al `PlanSaaSDto.cs`
3. Crear migración EF: `dotnet ef migrations add NombreMigracion`
4. Actualizar los valores en la BD

---

## Ver el estado actual de los planes

```bash
# Desde TestToken/Program.cs o via psql
SELECT "Id", "Nombre", "Precio", "MaxAtletas", "ResultadosTiempoReal", "ExportacionExcel", "SoportePrioritario"
FROM catalogos."PlanesSaaS"
ORDER BY "Id";
```
