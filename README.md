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

## Ejecutable independiente

- El archivo `output/CS2Affinity.exe` es un ejecutable **autocontenido** (publicado como single-file, self-contained). Simplemente copia ese `.exe` a otra máquina con Windows y ejecútalo; no requiere instalar .NET por separado.
- Para aplicar afinidad y prioridad a procesos es necesario ejecutar como **Administrador** (si no se ejecuta elevado la aplicación mostrará un aviso pero seguirá funcionando en modo limitado).
- Si la aplicación inicia Steam, esperará a que Steam esté completamente listo antes de lanzar CS2 para evitar errores del SDK.

## Subir a GitHub y publicar el EXE

Para que otros puedan descargar el `.exe` desde GitHub lo más cómodo es usar **GitHub Releases**. Pasos básicos:

1. Crea un repositorio nuevo en GitHub y empuja el código (`git remote add origin ...; git push --set-upstream origin main`).
2. Crea una etiqueta para la versión, por ejemplo `v1.0.0`: `git tag v1.0.0 && git push origin v1.0.0`.
3. El workflow de GitHub Actions (`.github/workflows/release.yml`) detectará la etiqueta, construirá el EXE (single-file, self-contained) y lo adjuntará automáticamente a la Release creada por la etiqueta.

También puedes hacer una Release manual desde la UI de GitHub y subir el `CS2Affinity.exe` que generes localmente con:

```powershell
dotnet publish CS2Affinity/CS2Affinity.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o output
# Sube output\CS2Affinity.exe a la Release en GitHub
```

### ¿Necesito `.replit`?

El archivo `.replit` pertenece al entorno Replit y **no es necesario** para que la aplicación funcione. Puedes mantenerlo si planeas usar Replit, o eliminarlo si no lo necesitas.

## Verificar el ejecutable descargado

Para que cualquiera pueda verificar que el `.exe` descargado desde una Release es el mismo que se ha publicado desde el código fuente, el workflow genera y adjunta un archivo `CS2Affinity.exe.sha256` a la Release. Para comprobarlo localmente:

1. Descarga `CS2Affinity.exe` y `CS2Affinity.exe.sha256` desde la Release.
2. En PowerShell, ejecuta:

```powershell
Get-FileHash -Algorithm SHA256 .\CS2Affinity.exe | Format-List
Get-Content .\CS2Affinity.exe.sha256 -Raw
```

3. Compara los hashes; deben coincidir.

También puedes generar el checksum localmente con el script incluido:

```powershell
.\scripts\generate_checksum.ps1
```

## Cómo puedo pasar el token (opciones seguras)

Si quieres que yo cree el repo y suba la Release por ti, tienes dos opciones seguras:

- **Invitarme como colaborador (recomendado):** crea el repo en tu cuenta y añádeme como colaborador con permisos de escritura. Yo haré el resto desde mi cuenta.
- **Compartir un PAT temporal:** genera un Personal Access Token (PAT) con scopes `repo` y `workflow`, con expiración corta (p. ej. 24h). Pégalo aquí en el chat sólo cuando estés listo y yo lo usaré para crear el repo, empujar `main`, crear la etiqueta `v1.0.0` (si lo deseas) y verificar que la Release contiene `CS2Affinity.exe` y su `.sha256`. Te recomiendo revocar el token tras la operación.

Si prefieres no compartir un token, puedo guiarte paso a paso para que lo hagas tú mismo sin darme acceso.

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

Desarrollado por **00bama**

## Licencia

Uso libre para fines personales.
