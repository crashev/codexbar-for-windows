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

## Odpowiedzi na najczęstsze pytania o upstream

### 1) Czy upstream używa `codex` CLI z PATH, czy API?
Upstream wspiera **kilka strategii źródła danych** i wybór zależy od providera oraz trybu:
- `auto` / `web`: dla części providerów preferowany jest web (cookies/session),
- `cli`: użycie lokalnych CLI (w tym Codex/Claude/Gemini),
- `oauth`: tam gdzie provider udostępnia flow OAuth,
- `api`: tam gdzie provider ma API token i endpoint usage.

W praktyce dla Codex upstream potrafi korzystać z lokalnego CLI (RPC/PTY fallback), ale ma też ścieżkę web dashboard dla dodatkowych danych.

### 2) Dlaczego w tym porcie jest tylko Codex?
To świadomy zakres MVP:
- szybkie uruchomienie stabilnej wersji Windows bez przepisywania całego upstream,
- pojedynczy parser i prosty tray UX do walidacji architektury,
- minimalizacja ryzyka (różni providerzy wymagają różnych auth flow: cookies, OAuth, API keys, lokalne CLI).

Wsparcie wielu providerów jest zaplanowane na kolejne iteracje po ustabilizowaniu rdzenia (refresh loop, parser, UI tray).

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
