# LogAnalysis

Aplicación de escritorio para analizar archivos de log en formato `.txt`, detectar información relevante de operación y generar un resumen exportable en CSV.

## Descripción

LogAnalysis permite seleccionar uno o varios archivos de log, procesarlos línea por línea y consolidar métricas por archivo, número de serie y versión encontrada. La interfaz muestra el avance del análisis, presenta los resultados en una tabla y permite exportarlos para abrirlos en Excel u otra herramienta compatible con CSV.

## Tecnologías

- C# con .NET 8
- Windows Forms
- Serilog para registro de eventos

## Requisitos

- Windows
- .NET 8 SDK o Visual Studio con soporte para .NET 8

## Cómo ejecutar

Desde la carpeta del proyecto:

```powershell
dotnet restore
dotnet build
dotnet run
```

También se puede abrir `LogAnalysis.sln` desde Visual Studio y ejecutar el proyecto `LogAnalysis`.

## Uso

1. Abrir la aplicación.
2. Presionar **Seleccionar**.
3. Elegir uno o varios archivos `.txt`.
4. Presionar **Analizar**.
5. Revisar los resultados en la tabla.
6. Presionar **Exportar CSV** para guardar el resumen.

## Información extraída

La aplicación consolida estos datos:

- Archivo analizado
- Número de serie
- Versión
- Fecha de primera aparición de la versión
- Cantidad de apariciones de la versión
- Fecha de última aparición de la versión
- Errores de cambio a modo depósito
- Operaciones de depósito con error
- Errores de almacenaje
- Depósitos exitosos
- Eventos de almacenaje
- Cantidad de usuarios diferentes
- Recolecciones realizadas

## Patrones reconocidos

El análisis busca expresiones como:

- `NUMERO DE SERIE ->`
- `versión`
- `NO FUE POSIBLE PASAR A MODO DEPOSITO`
- `TERMINO DE DEPOSITO CON ERROR`
- `Error durante el almacenaje`
- `Deposito exitoso`
- `Deposito enviado a CIRREON con exito`
- `TERMINO DE RECOLECCION`

Las fechas se esperan al inicio de línea con este formato:

```text
yyyy-MM-dd HH:mm:ss,fff
```

## Salida

El archivo exportado es un CSV con codificación UTF-8. Por defecto se propone el nombre:

```text
resultados.csv
```

## Logs de la aplicación

La aplicación escribe sus propios logs en la carpeta:

```text
logs/
```

Los archivos se rotan diariamente y se conservan hasta 15 archivos.

## Estructura del proyecto

```text
LogAnalysis/
├── Models/      Modelos usados por el análisis y los eventos de progreso
├── Services/    Lógica principal de procesamiento de archivos
├── Utils/       Funciones auxiliares para extracción y búsqueda de texto
├── frmMain.cs   Formulario principal de la aplicación
└── Program.cs   Punto de entrada y configuración de Serilog
```

## Notas

- Los archivos vacíos o inexistentes se omiten durante el análisis.
- El progreso avanza por archivo seleccionado, incluso cuando un archivo se omite por no ser válido.
- La búsqueda de frases ignora diferencias de mayúsculas, minúsculas y acentos.
