# CS2 Affinity

Optimizador de rendimiento para Counter-Strike 2 que aplica automáticamente configuraciones de afinidad de CPU y prioridad de proceso.

## Características

- Inicia Steam automáticamente si no está abierto
- Ejecuta CS2 usando la ruta directa para un arranque más rápido
- Establece prioridad del proceso a **HIGH**
- Desactiva CPU0 para mejorar el rendimiento (evita interrupciones del sistema)
- Se cierra automáticamente después de aplicar las optimizaciones

## Requisitos

- Windows 10/11 (64-bit)
- Steam instalado
- Counter-Strike 2 instalado

## Uso

1. Descargar `CS2Affinity.exe`
2. Ejecutar como **Administrador** (obligatorio)
3. La aplicación iniciará Steam y CS2 automáticamente
4. Las optimizaciones se aplicarán durante 30 segundos
5. La aplicación se cerrará automáticamente

## Nota sobre Windows SmartScreen

Al ser una aplicación sin firma digital, Windows puede mostrar una advertencia. Para ejecutar:

1. Click en "Más información"
2. Click en "Ejecutar de todos modos"

O alternativamente:
1. Click derecho en el archivo → Propiedades
2. Marcar "Desbloquear" → Aplicar → Aceptar

## Compilación

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Créditos

Desarrollado por **touch grass**

## Licencia

Uso libre para fines personales.
