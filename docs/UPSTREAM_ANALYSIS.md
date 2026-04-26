# Analiza upstream `steipete/codexbar`

## Struktura repozytorium
Upstream jest projektem Swift Package z kilkoma targetami:
- `Sources/CodexBar` – główna aplikacja menu bar (macOS UI + provider orchestration),
- `Sources/CodexBarCore` – core logiki pobierania danych i integracji (np. OpenAI Web),
- `Sources/CodexBarCLI` – CLI (`codexbar`) do odczytu usage/cost,
- `Sources/CodexBarWidget` – widget,
- `docs/` – dokumentacja providerów, architektury i CLI.

## Logika aplikacji
Kluczowa logika upstream:
1. Polling providerów w pętli odświeżania.
2. Znormalizowany model usage (`primary`, `secondary`, opcjonalnie `tertiary`).
3. Render wskaźników i resetów per provider.
4. Opcjonalny tryb merge icons i status pages.
5. Równoległe źródła danych: web, cli, oauth, api.

## Założenia portu Windows
Port w tym repo implementuje MVP w oparciu o te same zasady:
- tray app + pętla odświeżania,
- model `primary/secondary` mapowany 1:1,
- preferowany transport przez `codexbar` CLI JSON,
- fallback mock (tryb developerski bez zależności).

## Cel kolejnych iteracji
- multi-provider UI i przełączanie providerów,
- GUI ustawień zamiast hardcodowanej konfiguracji,
- ikonografia metrowa analogiczna do macOS.
