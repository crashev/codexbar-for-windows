# CodexBar for Windows (Win10+)

Nieoficjalny port aplikacji **CodexBar** na Windows 10/11.

Aplikacja działa jako tray app (ikona w zasobniku systemowym), odczytuje usage przez `codexbar` CLI i pokazuje:
- poziom zużycia dla okna sesyjnego (5h),
- poziom tygodniowy,
- czas resetu,
- źródło danych oraz stan odświeżenia.

## Dlaczego taki port?
Oryginalny projekt `steipete/codexbar` jest aplikacją macOS (SwiftUI + menu bar). Ten repozytorium implementuje analogiczny UX dla Windows:
- tray icon zamiast menu bar item,
- menu kontekstowe z podsumowaniem,
- automatyczny refresh,
- fallback na mock provider gdy `codexbar` nie jest zainstalowany.

## Wymagania
- Windows 10/11
- .NET 8 SDK (do builda)
- (Opcjonalnie) `codexbar` w PATH

## Skąd wziąć `dotnet` (SDK)?
Najprościej zainstalować .NET SDK 8 na Windows jedną z metod:

### Opcja 1: Winget (rekomendowane)
```powershell
winget install Microsoft.DotNet.SDK.8
```

### Opcja 2: Oficjalny instalator Microsoft
1. Wejdź na: https://dotnet.microsoft.com/download/dotnet/8.0
2. Pobierz **.NET SDK** dla Windows x64.
3. Zainstaluj i otwórz nowy terminal.

### Sprawdzenie instalacji
```powershell
dotnet --info
```
Jeżeli komenda działa i pokazuje wersję SDK, możesz uruchamiać projekt.

## Szybki start
```powershell
cd src\WinCodexBar
dotnet run -c Release
```

## Publikacja single-file (Windows)
```powershell
cd src\WinCodexBar
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

## Integracja z upstream `codexbar`
Aplikacja cyklicznie wykonuje:
```bash
codexbar --provider codex --format json --pretty
```

Parser przyjmuje zarówno pojedynczy obiekt JSON, jak i tablicę obiektów (gdy CLI zwraca wiele providerów).

## Aktualny zakres portu
- [x] Tray app z odświeżaniem co 60s
- [x] Render paska zużycia w tooltipie/menu
- [x] Obsługa primary + secondary usage window
- [x] Normalizacja danych z JSON CLI
- [x] Fallback mock data (bez CLI)
- [ ] Konfiguracja wielodostawców (Claude/Cursor/Gemini itd.)
- [ ] Widok ustawień GUI
- [ ] Zaawansowane ikony metrowe jak w macOS

## Struktura
- `src/WinCodexBar/Program.cs` – bootstrap aplikacji
- `src/WinCodexBar/AppContext/TrayAppContext.cs` – logika tray + refresh loop
- `src/WinCodexBar/Providers/*` – dostawcy danych usage
- `src/WinCodexBar/Models/*` – modele domenowe

## Licencja
MIT
