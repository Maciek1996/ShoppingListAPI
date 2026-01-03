# ShoppingListAPI

API do zarządzania listami zakupów — backend zbudowany w ASP.NET Core 3.1 + Entity Framework Core.  
README został przygotowany na podstawie przeglądu kodu w repozytorium (m.in. `Startup.cs`, `Program.cs`, `DbContexts`, kontrolery, DTO i encje).

Link do repozytorium / przeszukiwania kodu: https://github.com/Maciek1996/ShoppingListAPI/search

Spis treści
- Informacje ogólne
- Stos technologiczny
- Uruchomienie lokalne
- Docker (Dockerfile + docker-compose)
- Migracje i baza danych
- API — pełne przykłady endpointów (request/response + curl)
- Dobre praktyki i uwagi bezpieczeństwa
- Co dalej / kontrybucja
- Licencja / kontakt

Informacje ogólne
Projekt implementuje CRUD produktów i tagów oraz operacje na listach zakupów (tworzenie list, dodawanie/usuń produktów na liście, oznaczanie jako kupione). Używa AutoMapper do mapowania DTO ↔ encje oraz EF Core do persystencji w SQL Server.

Stos technologiczny
- .NET Core 3.1 (netcoreapp3.1)
- ASP.NET Core Web API
- EF Core 5.0.9 + Microsoft.EntityFrameworkCore.SqlServer
- AutoMapper
- CORS włączone globalnie

Uruchomienie lokalne (szybki start)
1. Sklonuj repo:
   git clone https://github.com/Maciek1996/ShoppingListAPI.git
2. Przejdź do katalogu:
   cd ShoppingListAPI
3. Przywróć zależności:
   dotnet restore
4. (Opcjonalnie) zainstaluj narzędzie EF:
   dotnet tool install --global dotnet-ef
5. Ustaw właściwy connection string (appsettings.json lub User Secrets / zmienne środowiskowe) — przykłady niżej.
6. Zastosuj migracje:
   dotnet ef database update
7. Uruchom:
   dotnet run --project ShoppingListAPI.csproj

Docker (Dockerfile i docker-compose)
Poniżej proponowany wieloetapowy Dockerfile oraz docker-compose.yml uruchamiający aplikację i SQL Server (kontener). Aplikacja odczyta połączenie z environment variable `ConnectionStrings__DefaultConnection`.

– Budowanie i uruchamianie lokalne przy pomocy Docker Compose:
- docker-compose up --build
- Aby zastosować migracje (z hosta): po uruchomieniu bazy uruchom `dotnet ef database update` wskazując kontener lub wejdź do kontenera aplikacji i uruchom polecenie migracji.

(Uwaga: SQL Server w kontenerze potrzebuje kilku sekund po starcie, przed uruchomieniem migracji; w produkcji użyj skryptu oczekiwania/healthcheck).

Migracje i baza danych
- W repo są migracje EF Core (Initial). Typowe komendy:
  - dotnet ef migrations add <Name>
  - dotnet ef database update

Jeżeli migracje mają być uruchamiane z kontenera:
- Opcja 1: Po uruchomieniu docker-compose, wejść do kontenera aplikacji i wykonać migracje:
  docker-compose exec web bash
  dotnet ef database update
- Opcja 2: Dodać inicjalizator / skrypt migracji w entrypoint aplikacji (wymaga obsługi oczekiwania na DB).

API — szczegółowe endpointy, JSON i przykłady curl
Poniższe przykłady odzwierciedlają DTO i logikę z kontrolerów w repo.

Enum QuantityType:
- 0 = None
- 1 = Piece
- 2 = Weight

1) Products
- GET /api/products?search={query}
  - Opis: pobiera wszystkie aktywne produkty; opcjonalne filtrowanie po nazwie
  - Przykład:
    curl http://localhost:5000/api/products
  - Response 200:
    [
      {
        "id": "11111111-1111-1111-1111-111111111111",
        "name": "Mleko",
        "description": "Mleko 2%",
        "type": 1
      }
    ]

- GET /api/products/{productId}
  - Response 200:
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "name": "Mleko",
      "description": "Mleko 2%",
      "type": 1
    }
  - Response 404: brak produktu

- POST /api/products
  - Body (ProductCreationDto):
    {
      "name": "Masło",
      "description": "Masło 200g",
      "type": 1
    }
  - Response 201 Created
    Location header → /api/products/{newId}
    Body (ProductDto):
    {
      "id": "22222222-2222-2222-2222-222222222222",
      "name": "Masło",
      "description": "Masło 200g",
      "type": 1
    }
  - Przykład curl:
    curl -X POST http://localhost:5000/api/products \
      -H "Content-Type: application/json" \
      -d '{"name":"Masło","description":"200g","type":1}'

- PUT /api/products/{productId}?changeForAll={true|false}
  - Body (ProductEditionDto):
    {
      "name": "Masło ekstra",
      "description": "Masło 250g",
      "type": 1
    }
  - Response: 204 NoContent

- DELETE /api/products/{productId}
  - Response 200 OK (jeśli usunięto lub oznaczono jako nieaktywne) lub 404

2) Tags
- GET /api/tags
  - Response 200:
    [
      {
        "id": "33333333-3333-3333-3333-333333333333",
        "tagName": "Tygodniowe"
      }
    ]

- GET /api/tags/{tagId}
  - Response 200: TagDto
  - Response 404: NotFound

- POST /api/tags
  - Body (TagCreationDto):
    {
      "tagName": "Tygodniowe"
    }
  - Response 201 Created
    Body (Tag entity as returned):
    {
      "id": "33333333-3333-3333-3333-333333333333",
      "tagName": "Tygodniowe",
      "isActive": true
    }

- PUT /api/tags/{tagId}
  - Body (TagEditionDto):
    {
      "tagName": "Nowa nazwa"
    }
  - Response 204 NoContent

- DELETE /api/tags/{tagId}
  - Response 200 OK lub 404

3) Shopping lists
- GET /api/list?tagId={guid}
  - Opis: zwraca aktualną listę (dla danego tagId jeśli podany)
  - Response 200:
    [
      {
        "productId": "11111111-1111-1111-1111-111111111111",
        "name": "Mleko",
        "description": "Mleko 2%",
        "isBought": false,
        "pieces": 1,
        "weight": 0.0,
        "unit": "kg",
        "type": 1
      }
    ]
  - Response 404: brak listy

- GET /api/lists
  - Zwraca listę obiektów ShoppingListDto
  - Example ShoppingListDto:
    {
      "id": "44444444-4444-4444-4444-444444444444",
      "creationDate": "2022-04-09T21:57:28",
      "tagName": "Tygodniowe",
      "productLists": [ ... ProductListDto ... ]
    }

- POST /api/list?tagId={guid}
  - Tworzy nową listę bazując na "aktualnej" i zwraca produkty nowej listy
  - Response 201 Created z tablicą ProductListDto
  - Response 404 z komunikatem, jeśli nie znaleziono listy lub lista pusta

- PUT /api/list/{productId}?tagId={guid}&pieces={int}&weight={double}
  - Dodaje pojedynczy produkt do aktualnej listy
  - Response:
    - 201 Created (jeśli dodano)
    - 400 BadRequest (jeśli już istnieje)
    - 404 NotFound (jeśli brak listy/produktu)

- DELETE /api/list/{productId}?tagId={guid}
  - Usuwa produkt z listy
  - Response: 200 OK lub 404 / 400

- PUT /api/list (body = IEnumerable<ProductListDto>)?tagId={guid}
  - Dodawanie wielu produktów na raz.
  - Body przykład:
    [
      {
        "productId": "11111111-1111-1111-1111-111111111111",
        "pieces": 2,
        "weight": 0.0,
        "type": 1
      }
    ]
  - Response 200 OK z aktualną listą (ShoppingListDto) lub 404

Przykłady curl (skrót)
- Pobierz produkty:
  curl http://localhost:5000/api/products
- Dodaj produkt:
  curl -X POST http://localhost:5000/api/products -H "Content-Type: application/json" -d '{"name":"Chleb","description":"Czarny","type":1}'
- Pobierz aktualną listę:
  curl "http://localhost:5000/api/list"

Dobre praktyki i uwagi bezpieczeństwa
- W repo znajduje się connection string z hasłem w `appsettings.json`. To wrażliwe dane — usuń/zmień je natychmiast przed publikacją. Używaj User Secrets lokalnie i zmiennych środowiskowych w CI/produkcji.
- EF Core 5 + .NET Core 3.1 — rozważ upgrade do dłużej wspieranego .NET 6/7.
- Ogranicz CORS do zaufanych originów w środowisku produkcyjnym.
- Przy wdrożeniu uwzględnij backup bazy i bezpieczeństwo haseł.

Co mogę zrobić dalej
- Wstawić te pliki do repo i utworzyć PR.
- Dodać skrypt inicializacji migracji w Docker Compose (skrypt "wait-for-db" + migracje automatyczne).
- Dodać dokumentację OpenAPI/Swagger (jeśli chcesz, mogę dodać konfigurację Swagger w Startup.cs i przykładowy endpoint /swagger).

Kontakt
- Repo: https://github.com/Maciek1996/ShoppingListAPI
- Autor: Maciek1996
